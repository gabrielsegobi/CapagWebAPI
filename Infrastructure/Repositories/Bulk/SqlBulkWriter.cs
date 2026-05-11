using Domain.Entities.Sped;
using Domain.Entities.Sped.Ecf;
using Infrastructure.Repositories.Bulk;
using MySqlConnector;
using System.Data;
using System.Diagnostics;
using System.Threading.Channels;

namespace Infrastructure.Writers.Sped
{
    /// <summary>
    /// Writer de alta performance para ECF.
    ///
    /// Diferença principal em relação à versão anterior:
    ///   - Usa EcfLayout em vez de RegistroLayout
    ///   - GetOrCreateDataTable usa FieldLayout.Coluna como nome da coluna no banco
    ///     (em vez de FieldLayout.Nome que é o nome no SPED)
    ///   - GetCampos(layoutNumber) para montar o schema correto por versão
    /// </summary>
    public class SqlBulkWriter : IBulkWriter
    {
        // ── configuração ─────────────────────────────────────────────
        private const int BATCH_SIZE = 20_000;
        private const int BULK_TIMEOUT_SEC = 0;

        // ── dependências ─────────────────────────────────────────────
        private readonly MySqlConnection _conn;
        private readonly int _layoutNumber;

        // Pré-indexado por tabela — O(1) por flush
        private readonly Dictionary<string, EcfLayout> _layoutsPorTabela;

        // ── estado interno ───────────────────────────────────────────
        private readonly Dictionary<string, List<ValueBuffer>> _batches = new();
        private readonly Dictionary<string, DataTable> _schemaCache = new();
        private int _pendingRows;

        // ── métricas ─────────────────────────────────────────────────
        private long _totalLinhas;
        private int _totalFlushes;
        private long _tempoTotalFlushMs;
        private bool _disposed;

        public BulkWriterMetrics Metrics => new()
        {
            TotalLinhasInseridas = _totalLinhas,
            TotalFlushes = _totalFlushes,
            TempoTotalFlushMs = _tempoTotalFlushMs
        };

        /// <param name="layoutNumber">
        /// Número do layout ECF — necessário para montar o DataTable
        /// com as colunas corretas via GetCampos(layoutNumber).
        /// </param>
        public SqlBulkWriter(
            MySqlConnection conn,
            Dictionary<string, EcfLayout> layouts,
            int layoutNumber)
        {
            _conn = conn;
            _layoutNumber = layoutNumber;

            _layoutsPorTabela = layouts.Values
                .Where(l => !string.IsNullOrWhiteSpace(l.Tabela))
                .GroupBy(l => l.Tabela)
                .ToDictionary(g => g.Key, g => g.First());
        }

        // ── Add: acumula e flush automático ──────────────────────────
        public async Task AddAsync(ParsedRow row, CancellationToken cancellationToken = default)
        {
            if (!_batches.TryGetValue(row.Tabela, out var lista))
            {
                lista = new List<ValueBuffer>(BATCH_SIZE);
                _batches[row.Tabela] = lista;
            }

            lista.Add(row.Buffer);
            _pendingRows++;

            // Agora o BATCH_SIZE é global: soma de todas as tabelas.
            // Quando atingir o limite, faz flush de todas as tabelas pendentes.
            if (_pendingRows >= BATCH_SIZE)
                await FlushAllAsync(cancellationToken);
        }

        // ── ConsumeAsync: consumer do Channel ────────────────────────
        public async Task ConsumeAsync(
            ChannelReader<ParsedRow> reader,
            CancellationToken cancellationToken = default)
        {
            var inicio = DateTime.Now;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await foreach (var row in reader.ReadAllAsync(cancellationToken))
                    await AddAsync(row, cancellationToken);

                await FlushAllAsync(cancellationToken);
            }
            finally
            {
                stopwatch.Stop();
                var fim = DateTime.Now;

                var segundos = stopwatch.Elapsed.TotalSeconds > 0
                    ? stopwatch.Elapsed.TotalSeconds
                    : 0.000001;

                var linhasPorSegundo = _totalLinhas / segundos;
                var mediaFlushMs = _totalFlushes > 0
                    ? (double)_tempoTotalFlushMs / _totalFlushes
                    : 0.0;
            }
        }

        // ── flush de uma tabela ───────────────────────────────────────
        private async Task FlushAsync(string tabela, CancellationToken cancellationToken)
        {
            if (!_batches.TryGetValue(tabela, out var lista) || lista.Count == 0)
            {
                Console.WriteLine($"[FLUSH SKIP] {tabela} — lista vazia");
                return;
            }

            if (!_layoutsPorTabela.TryGetValue(tabela, out var layout))
            {
                DevolverArrays(lista);
                lista.Clear();
                return;
            }

            var sw = Stopwatch.StartNew();

            var dt = GetOrCreateDataTable(tabela, layout);
            dt.Rows.Clear();

            foreach (var buf in lista)
            {
                dt.Rows.Add(buf.Values.AsSpan(0, buf.FieldCount).ToArray());
            }

            var bulk = new MySqlBulkCopy(_conn)
            {
                DestinationTableName = tabela,
                BulkCopyTimeout = BULK_TIMEOUT_SEC
            };

            // MySqlBulkCopy usa ColumnMappings com índice ordinal
            for (int i = 0; i < dt.Columns.Count; i++)
                bulk.ColumnMappings.Add(new MySqlBulkCopyColumnMapping(i, dt.Columns[i].ColumnName));

            //await bulk.WriteToServerAsync(dt, cancellationToken);
            try
            {
                await bulk.WriteToServerAsync(dt, cancellationToken);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("====== MYSQL EXCEPTION ======");
                Console.WriteLine($"Number: {ex.Number}");
                Console.WriteLine($"SqlState: {ex.SqlState}");
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine($"Inner: {ex.InnerException?.Message}");
                Console.WriteLine("=============================");

                throw; // rethrow para não mascarar
            }
            sw.Stop();

            // Devolve arrays ao pool após inserção bem-sucedida
            DevolverArrays(lista);

            // Guarda count antes de limpar
            var count = lista.Count;
            lista.Clear();
            
            _totalLinhas += count;
            _totalFlushes++;
            _tempoTotalFlushMs += sw.ElapsedMilliseconds;
        }

        // ── devolve arrays ao pool ────────────────────────────────────
        private static void DevolverArrays(List<ValueBuffer> lista)
        {
            foreach (var buf in lista)
            {

                var b = buf;
                b.Return();
            }
        }

        public async Task FlushAllAsync(CancellationToken cancellationToken = default)
        {
            foreach (var tabela in _batches.Keys.ToList())
                await FlushAsync(tabela, cancellationToken);

            // Após flush global, não há mais linhas pendentes em memória.
            _pendingRows = 0;
        }

        // ── monta DataTable com colunas corretas para o layoutNumber ──
        /// <summary>
        /// Usa FieldLayout.Coluna como nome da coluna no banco.
        /// Ex: campo com Nome="CNPJ" e Coluna="cnpj" → coluna "cnpj" no DataTable.
        /// O schema é cacheado — criado uma única vez por tabela por instância.
        /// </summary>
        private DataTable GetOrCreateDataTable(string tabela, EcfLayout layout)
        {
            if (_schemaCache.TryGetValue(tabela, out var cached))
                return cached;

            var campos = layout.GetCampos(_layoutNumber);
            var dt = new DataTable(tabela);

            // Colunas fixas — sempre as mesmas, ordem igual ao ValueBuffer
            dt.Columns.Add("id", typeof(long));
            dt.Columns.Add("id_pai", typeof(long));
            dt.Columns.Add("file_id", typeof(long));
            dt.Columns.Add("id_op", typeof(long));
            //dt.Columns.Add("IdTenant", typeof(long));
            //dt.Columns.Add("IdEmpresa", typeof(long));
            //dt.Columns.Add("Competencia", typeof(string));

            // Colunas dinâmicas — usa Coluna (nome no banco), não Nome (nome SPED)
            foreach (var campo in campos)
            {
                var tipo = campo.Tipo switch
                {
                    "decimal" => typeof(decimal),
                    "int" => typeof(int),
                    "long" => typeof(long),
                    "date" => typeof(DateTime),
                    _ => typeof(string)
                };

                // Usa campo.Coluna se preenchido, senão campo.Nome como fallback
                var nomeColuna = string.IsNullOrWhiteSpace(campo.Coluna)
                    ? campo.Nome
                    : campo.Coluna;

                dt.Columns.Add(nomeColuna, tipo);
            }

            _schemaCache[tabela] = dt;
            return dt;
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;
            await FlushAllAsync(CancellationToken.None);
        }
    }
}