//using Domain.Entities;
//using Domain.Entities.Sped;
//using Infrastructure.Repositories.Bulk;
//using Microsoft.Data.SqlClient;
//using System.Buffers;
//using System.Data;
//using System.Diagnostics;
//using System.Threading.Channels;

//namespace Infrastructure.Writers.Sped
//{
//    /// <summary>
//    /// Etapa 6: SqlBulkWriter com ArrayPool.
//    ///
//    /// Diferença da versão Etapa 4:
//    ///   - Usa ValueBuffer.Rent() em vez de new ValueBuffer()
//    ///   - Devolve arrays ao pool após cada flush (Return())
//    ///   - Pré-indexa layouts por tabela em vez de FirstOrDefault() por flush
//    ///   - Resultado: GC Gen1/Gen2 praticamente zero durante processamento
//    ///
//    /// O parser deve usar ValueBuffer.Rent() para criar os buffers.
//    /// O writer chama Return() em cada buffer após inserção no banco.
//    /// </summary>
//    public class SqlBulkWriter : IBulkWriter
//    {
//        // ── configuração ─────────────────────────────────────────────
//        private const int BATCH_SIZE = 5_000;
//        private const int BULK_TIMEOUT_SEC = 120;

//        // ── dependências ─────────────────────────────────────────────
//        private readonly SqlConnection _conn;

//        // Etapa 6: pré-indexado por tabela — evita FirstOrDefault() por flush
//        private readonly Dictionary<string, RegistroLayout> _layoutsPorTabela;

//        // ── estado interno ───────────────────────────────────────────
//        private readonly Dictionary<string, List<ValueBuffer>> _batches = new();
//        private readonly Dictionary<string, DataTable> _schemaCache = new();

//        // ── métricas ─────────────────────────────────────────────────
//        private long _totalLinhas;
//        private int _totalFlushes;
//        private long _tempoTotalFlushMs;
//        private bool _disposed;

//        public BulkWriterMetrics Metrics => new()
//        {
//            TotalLinhasInseridas = _totalLinhas,
//            TotalFlushes = _totalFlushes,
//            TempoTotalFlushMs = _tempoTotalFlushMs
//        };

//        public SqlBulkWriter(
//            SqlConnection conn,
//            Dictionary<string, RegistroLayout> layouts)
//        {
//            _conn = conn;

//            // Etapa 6: índice por tabela — O(1) por flush em vez de O(n)
//            _layoutsPorTabela = layouts.Values
//                .Where(l => !string.IsNullOrWhiteSpace(l.Tabela))
//                .GroupBy(l => l.Tabela)
//                .ToDictionary(g => g.Key, g => g.First());
//        }

//        // ── Add: acumula e faz flush automático ───────────────────────
//        public async Task AddAsync(ParsedRow row, CancellationToken cancellationToken = default)
//        {
//            if (!_batches.TryGetValue(row.Tabela, out var lista))
//            {
//                lista = new List<ValueBuffer>(BATCH_SIZE);
//                _batches[row.Tabela] = lista;
//            }

//            lista.Add(row.Buffer);

//            if (lista.Count >= BATCH_SIZE)
//                await FlushAsync(row.Tabela, cancellationToken);
//        }

//        // ── ConsumeAsync: loop do Channel (Etapa 5) ──────────────────
//        public async Task ConsumeAsync(
//            ChannelReader<ParsedRow> reader,
//            CancellationToken cancellationToken = default)
//        {
//            await foreach (var row in reader.ReadAllAsync(cancellationToken))
//                await AddAsync(row, cancellationToken);

//            await FlushAllAsync(cancellationToken);
//        }

//        // ── flush de uma tabela ───────────────────────────────────────
//        private async Task FlushAsync(string tabela, CancellationToken cancellationToken)
//        {
//            if (!_batches.TryGetValue(tabela, out var lista) || lista.Count == 0)
//                return;

//            if (!_layoutsPorTabela.TryGetValue(tabela, out var layout))
//            {
//                // Tabela não mapeada — devolve arrays e limpa
//                DevolverArrays(lista);
//                lista.Clear();
//                return;
//            }

//            var sw = Stopwatch.StartNew();

//            var dt = GetOrCreateDataTable(tabela, layout);
//            dt.Rows.Clear();

//            foreach (var buf in lista)
//                dt.Rows.Add(buf.Values);

//            using var bulk = new SqlBulkCopy(_conn)
//            {
//                DestinationTableName = tabela,
//                BatchSize = lista.Count,
//                BulkCopyTimeout = BULK_TIMEOUT_SEC
//            };

//            foreach (DataColumn col in dt.Columns)
//                bulk.ColumnMappings.Add(col.ColumnName, col.ColumnName);

//            await bulk.WriteToServerAsync(dt, cancellationToken);

//            sw.Stop();

//            // Etapa 6: devolve arrays ao pool APÓS inserção bem-sucedida
//            DevolverArrays(lista);
//            lista.Clear();

//            _totalLinhas += lista.Count;
//            _totalFlushes++;
//            _tempoTotalFlushMs += sw.ElapsedMilliseconds;
//        }

//        // ── Etapa 6: devolve todos os arrays do batch ao pool ─────────
//        private static void DevolverArrays(List<ValueBuffer> lista)
//        {
//            foreach (var buf in lista)
//            {
//                var b = buf; // cópia da struct para chamar método
//                b.Return();
//            }
//        }

//        public async Task FlushAllAsync(CancellationToken cancellationToken = default)
//        {
//            foreach (var tabela in _batches.Keys.ToList())
//                await FlushAsync(tabela, cancellationToken);
//        }

//        // ── schema cache por tabela ───────────────────────────────────
//        private DataTable GetOrCreateDataTable(string tabela, RegistroLayout layout)
//        {
//            if (_schemaCache.TryGetValue(tabela, out var cached))
//                return cached;

//            var dt = new DataTable(tabela);

//            dt.Columns.Add("Id", typeof(long));
//            dt.Columns.Add("IdPai", typeof(long));
//            dt.Columns.Add("FileId", typeof(long));
//            dt.Columns.Add("IdOp", typeof(long));
//            dt.Columns.Add("IdTenant", typeof(long));
//            dt.Columns.Add("IdEmpresa", typeof(long));
//            dt.Columns.Add("Competencia", typeof(string));

//            foreach (var campo in layout.Campos)
//            {
//                var tipo = campo.Tipo switch
//                {
//                    "decimal" => typeof(decimal),
//                    "int" => typeof(int),
//                    "long" => typeof(long),
//                    "date" => typeof(DateTime),
//                    _ => typeof(string)
//                };
//                dt.Columns.Add(campo.Nome, tipo);
//            }

//            _schemaCache[tabela] = dt;
//            return dt;
//        }

//        public void ImprimirMetricas()
//        {
//            Console.WriteLine($"\n[BULK WRITER V2 MÉTRICAS]");
//            Console.WriteLine($"  Total inserido:    {_totalLinhas:N0} linhas");
//            Console.WriteLine($"  Total flushes:     {_totalFlushes:N0}");
//            Console.WriteLine($"  Tempo total banco: {_tempoTotalFlushMs:N0}ms");
//            if (_totalFlushes > 0)
//                Console.WriteLine($"  Média por flush:   {_tempoTotalFlushMs / _totalFlushes:N0}ms");
//        }

//        public async ValueTask DisposeAsync()
//        {
//            if (_disposed) return;
//            _disposed = true;
//            await FlushAllAsync(CancellationToken.None);
//        }
//    }
//}


//using Domain.Entities.Sped;
//using Domain.Entities.Sped.Ecf;
//using Infrastructure.Repositories.Bulk;
//using Microsoft.Data.SqlClient;
//using System.Data;
//using System.Diagnostics;
//using System.Threading.Channels;

//namespace Infrastructure.Writers.Sped
//{
//    /// <summary>
//    /// Writer de alta performance para ECF.
//    ///
//    /// Diferença principal em relação à versão anterior:
//    ///   - Usa EcfLayout em vez de RegistroLayout
//    ///   - GetOrCreateDataTable usa FieldLayout.Coluna como nome da coluna no banco
//    ///     (em vez de FieldLayout.Nome que é o nome no SPED)
//    ///   - GetCampos(layoutNumber) para montar o schema correto por versão
//    /// </summary>
//    public class SqlBulkWriter : IBulkWriter
//    {
//        // ── configuração ─────────────────────────────────────────────
//        private const int BATCH_SIZE = 5_000;
//        private const int BULK_TIMEOUT_SEC = 120;

//        // ── dependências ─────────────────────────────────────────────
//        private readonly SqlConnection _conn;
//        private readonly int _layoutNumber;

//        // Pré-indexado por tabela — O(1) por flush
//        private readonly Dictionary<string, EcfLayout> _layoutsPorTabela;

//        // ── estado interno ───────────────────────────────────────────
//        private readonly Dictionary<string, List<ValueBuffer>> _batches = new();
//        private readonly Dictionary<string, DataTable> _schemaCache = new();

//        // ── métricas ─────────────────────────────────────────────────
//        private long _totalLinhas;
//        private int _totalFlushes;
//        private long _tempoTotalFlushMs;
//        private bool _disposed;

//        public BulkWriterMetrics Metrics => new()
//        {
//            TotalLinhasInseridas = _totalLinhas,
//            TotalFlushes = _totalFlushes,
//            TempoTotalFlushMs = _tempoTotalFlushMs
//        };

//        /// <param name="layoutNumber">
//        /// Número do layout ECF — necessário para montar o DataTable
//        /// com as colunas corretas via GetCampos(layoutNumber).
//        /// </param>
//        public SqlBulkWriter(
//            SqlConnection conn,
//            Dictionary<string, EcfLayout> layouts,
//            int layoutNumber)
//        {
//            _conn = conn;
//            _layoutNumber = layoutNumber;

//            _layoutsPorTabela = layouts.Values
//                .Where(l => !string.IsNullOrWhiteSpace(l.Tabela))
//                .GroupBy(l => l.Tabela)
//                .ToDictionary(g => g.Key, g => g.First());
//        }

//        // ── Add: acumula e flush automático ──────────────────────────
//        public async Task AddAsync(ParsedRow row, CancellationToken cancellationToken = default)
//        {
//            if (!_batches.TryGetValue(row.Tabela, out var lista))
//            {
//                lista = new List<ValueBuffer>(BATCH_SIZE);
//                _batches[row.Tabela] = lista;
//            }

//            lista.Add(row.Buffer);

//            if (lista.Count >= BATCH_SIZE)
//                await FlushAsync(row.Tabela, cancellationToken);
//        }

//        // ── ConsumeAsync: consumer do Channel ────────────────────────
//        public async Task ConsumeAsync(
//            ChannelReader<ParsedRow> reader,
//            CancellationToken cancellationToken = default)
//        {
//            await foreach (var row in reader.ReadAllAsync(cancellationToken))
//                await AddAsync(row, cancellationToken);

//            await FlushAllAsync(cancellationToken);
//        }

//        // ── flush de uma tabela ───────────────────────────────────────
//        private async Task FlushAsync(string tabela, CancellationToken cancellationToken)
//        {
//            if (!_batches.TryGetValue(tabela, out var lista) || lista.Count == 0)
//                return;

//            if (!_layoutsPorTabela.TryGetValue(tabela, out var layout))
//            {
//                DevolverArrays(lista);
//                lista.Clear();
//                return;
//            }

//            var sw = Stopwatch.StartNew();

//            var dt = GetOrCreateDataTable(tabela, layout);
//            dt.Rows.Clear();

//            foreach (var buf in lista)
//                dt.Rows.Add(buf.Values);



//            using var bulk = new SqlBulkCopy(_conn)
//            {
//                DestinationTableName = tabela,
//                BatchSize = lista.Count,
//                BulkCopyTimeout = BULK_TIMEOUT_SEC
//            };

//            foreach (DataColumn col in dt.Columns)
//                bulk.ColumnMappings.Add(col.ColumnName, col.ColumnName);

//            await bulk.WriteToServerAsync(dt, cancellationToken);

//            sw.Stop();

//            // Devolve arrays ao pool após inserção bem-sucedida
//            DevolverArrays(lista);

//            // Guarda count antes de limpar
//            var count = lista.Count;
//            lista.Clear();

//            _totalLinhas += count;
//            _totalFlushes++;
//            _tempoTotalFlushMs += sw.ElapsedMilliseconds;
//        }

//        // ── devolve arrays ao pool ────────────────────────────────────
//        private static void DevolverArrays(List<ValueBuffer> lista)
//        {
//            foreach (var buf in lista)
//            {
//                var b = buf;
//                b.Return();
//            }
//        }

//        public async Task FlushAllAsync(CancellationToken cancellationToken = default)
//        {
//            foreach (var tabela in _batches.Keys.ToList())
//                await FlushAsync(tabela, cancellationToken);
//        }

//        // ── monta DataTable com colunas corretas para o layoutNumber ──
//        /// <summary>
//        /// Usa FieldLayout.Coluna como nome da coluna no banco.
//        /// Ex: campo com Nome="CNPJ" e Coluna="cnpj" → coluna "cnpj" no DataTable.
//        /// O schema é cacheado — criado uma única vez por tabela por instância.
//        /// </summary>
//        private DataTable GetOrCreateDataTable(string tabela, EcfLayout layout)
//        {
//            if (_schemaCache.TryGetValue(tabela, out var cached))
//                return cached;

//            var campos = layout.GetCampos(_layoutNumber);
//            var dt = new DataTable(tabela);

//            // Colunas fixas — sempre as mesmas, ordem igual ao ValueBuffer
//            dt.Columns.Add("Id", typeof(long));
//            dt.Columns.Add("IdPai", typeof(long));
//            dt.Columns.Add("FileId", typeof(long));
//            dt.Columns.Add("IdOp", typeof(long));
//            //dt.Columns.Add("IdTenant", typeof(long));
//            //dt.Columns.Add("IdEmpresa", typeof(long));
//            //dt.Columns.Add("Competencia", typeof(string));

//            // Colunas dinâmicas — usa Coluna (nome no banco), não Nome (nome SPED)
//            foreach (var campo in campos)
//            {
//                var tipo = campo.Tipo switch
//                {
//                    "decimal" => typeof(decimal),
//                    "int" => typeof(int),
//                    "long" => typeof(long),
//                    "date" => typeof(DateTime),
//                    _ => typeof(string)
//                };

//                // Usa campo.Coluna se preenchido, senão campo.Nome como fallback
//                var nomeColuna = string.IsNullOrWhiteSpace(campo.Coluna)
//                    ? campo.Nome
//                    : campo.Coluna;

//                dt.Columns.Add(nomeColuna, tipo);
//            }

//            _schemaCache[tabela] = dt;
//            return dt;
//        }

//        public void ImprimirMetricas()
//        {
//            Console.WriteLine($"\n[BULK WRITER MÉTRICAS]");
//            Console.WriteLine($"  Total inserido:    {_totalLinhas:N0} linhas");
//            Console.WriteLine($"  Total flushes:     {_totalFlushes:N0}");
//            Console.WriteLine($"  Tempo total banco: {_tempoTotalFlushMs:N0}ms");
//            if (_totalFlushes > 0)
//                Console.WriteLine($"  Média por flush:   {_tempoTotalFlushMs / _totalFlushes:N0}ms");
//        }

//        public async ValueTask DisposeAsync()
//        {
//            if (_disposed) return;
//            _disposed = true;
//            await FlushAllAsync(CancellationToken.None);
//        }
//    }
//}


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
            Console.WriteLine($"\n[WRITER] Início ConsumeAsync  Horário={inicio:O}");

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

                Console.WriteLine($"\n[WRITER] Fim ConsumeAsync    Horário={fim:O}");
                Console.WriteLine($"[WRITER] Duração total: {stopwatch.Elapsed.TotalSeconds:N2}s ({stopwatch.ElapsedMilliseconds} ms)");
                Console.WriteLine($"[WRITER] Total inserido:    {_totalLinhas:N0} linhas em {_totalFlushes:N0} flushes");
                Console.WriteLine($"[WRITER] Throughput aprox.: {linhasPorSegundo:N0} linhas/segundo");
                Console.WriteLine($"[WRITER] Tempo médio por flush: {mediaFlushMs:N2} ms");
            }
        }

        // ── flush de uma tabela ───────────────────────────────────────
        private async Task FlushAsync(string tabela, CancellationToken cancellationToken)
        {
             //Console.WriteLine($"FLUSH CHAMADO: {tabela} ");

            if (!_batches.TryGetValue(tabela, out var lista) || lista.Count == 0)
            {
                 //Console.WriteLine($"[FLUSH SKIP] {tabela} — lista vazia");
                return;
            }
            // Console.WriteLine($"[FLUSH START] {tabela} | Count: {lista.Count} | IDs: {string.Join(",", lista.Select(b => b.Id))}");

            //Console.WriteLine($"FLUSH CHAMADO: {tabela} | Count: {lista.Count}");

            if (!_layoutsPorTabela.TryGetValue(tabela, out var layout))
            {
                DevolverArrays(lista);
                lista.Clear();
                return;
            }

            var sw = Stopwatch.StartNew();

            var dt = GetOrCreateDataTable(tabela, layout);
             dt.Rows.Clear();
            //Console.WriteLine($"\n[FLUSH] Tabela: {tabela}");
            //Console.WriteLine($"Colunas ({dt.Columns.Count}):");

            //foreach (DataColumn col in dt.Columns)
            //Console.Write($"{col.ColumnName} | ");

            //Console.WriteLine("\n--- Linhas ---");

            //foreach (DataRow row in dt.Rows)
            //{
            //    for (int i = 0; i < dt.Columns.Count; i++)
            //        Console.Write($"{row[i]} | ");

            //    Console.WriteLine();
            //}
            //foreach (var buf in lista)
            //    dt.Rows.Add(buf.Values);
            foreach (var buf in lista)
            {
                //Console.WriteLine("\n[BUFFER]");
                //for (int i = 0; i < buf.FieldCount; i++)
                //{
                //    var nomeColuna = dt.Columns[i].ColumnName;
                //    var valor = buf.Values[i];
                //    Console.WriteLine($"{nomeColuna} = {valor}");
                //}
                // ArrayPool.Rent(n) retorna array com tamanho MAIOR que n.
                // Ex: Rent(20) pode retornar array de tamanho 32.
                // DataTable com 20 colunas recebe array de 32 → crash.
                // buf.FieldCount guarda o tamanho real solicitado no Rent().
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

        public void ImprimirMetricas()
        {
            Console.WriteLine($"\n[BULK WRITER MÉTRICAS]");
            Console.WriteLine($"  Total inserido:    {_totalLinhas:N0} linhas");
            Console.WriteLine($"  Total flushes:     {_totalFlushes:N0}");
            Console.WriteLine($"  Tempo total banco: {_tempoTotalFlushMs:N0}ms");
            if (_totalFlushes > 0)
                Console.WriteLine($"  Média por flush:   {_tempoTotalFlushMs / _totalFlushes:N0}ms");
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;
            await FlushAllAsync(CancellationToken.None);
        }
    }
}




//using Domain.Entities.Sped;
//using Domain.Entities.Sped.Ecf;
//using Infrastructure.Repositories.Bulk;
//using MySqlConnector;
//using System.Data;
//using System.Diagnostics;
//using System.Threading.Channels;

//namespace Infrastructure.Writers.Sped
//{
//    public class SqlBulkWriter : IBulkWriter
//    {
//        private const int BATCH_SIZE = 20_000;
//        private const int BULK_TIMEOUT_SEC = 0;

//        private readonly MySqlConnection _conn;
//        private readonly int _layoutNumber;

//        private readonly Dictionary<string, EcfLayout> _layoutsPorTabela;
//        private readonly Dictionary<string, List<ValueBuffer>> _batches = new();

//        // Guarda só o SCHEMA (estrutura de colunas), não o DataTable com linhas.
//        // A cada flush clonamos o schema para um DataTable novo e limpo.
//        private readonly Dictionary<string, DataTable> _schemaCache = new();

//        private long _totalLinhas;
//        private int _totalFlushes;
//        private long _tempoTotalFlushMs;
//        private bool _disposed;

//        public BulkWriterMetrics Metrics => new()
//        {
//            TotalLinhasInseridas = _totalLinhas,
//            TotalFlushes = _totalFlushes,
//            TempoTotalFlushMs = _tempoTotalFlushMs
//        };

//        public SqlBulkWriter(
//            MySqlConnection conn,
//            Dictionary<string, EcfLayout> layouts,
//            int layoutNumber)
//        {
//            _conn = conn;
//            _layoutNumber = layoutNumber;

//            _layoutsPorTabela = layouts.Values
//                .Where(l => !string.IsNullOrWhiteSpace(l.Tabela))
//                .GroupBy(l => l.Tabela)
//                .ToDictionary(g => g.Key, g => g.First());
//        }

//        public async Task AddAsync(ParsedRow row, CancellationToken cancellationToken = default)
//        {
//            if (!_batches.TryGetValue(row.Tabela, out var lista))
//            {
//                lista = new List<ValueBuffer>(BATCH_SIZE);
//                _batches[row.Tabela] = lista;
//            }

//            lista.Add(row.Buffer);

//            if (lista.Count >= BATCH_SIZE)
//                await FlushAsync(row.Tabela, cancellationToken);
//        }

//        public async Task ConsumeAsync(
//            ChannelReader<ParsedRow> reader,
//            CancellationToken cancellationToken = default)
//        {
//            var sw = Stopwatch.StartNew();
//            try
//            {
//                await foreach (var row in reader.ReadAllAsync(cancellationToken))
//                    await AddAsync(row, cancellationToken);

//                await FlushAllAsync(cancellationToken);
//            }
//            finally
//            {
//                sw.Stop();
//                Console.WriteLine($"[WRITER] {sw.Elapsed.TotalSeconds:N2}s ({sw.ElapsedMilliseconds}ms)");
//            }
//        }

//        private async Task FlushAsync(string tabela, CancellationToken cancellationToken)
//        {
//            if (!_batches.TryGetValue(tabela, out var lista) || lista.Count == 0)
//                return;

//            if (!_layoutsPorTabela.TryGetValue(tabela, out var layout))
//            {
//                DevolverArrays(lista);
//                lista.Clear();
//                return;
//            }

//            var sw = Stopwatch.StartNew();
//            var count = lista.Count; // salva ANTES de qualquer operacao

//            // Clona o schema — DataTable novo e limpo a cada flush.
//            // Evita que linhas residuais do flush anterior sejam reenviadas.
//            var dt = ObterSchemaClonado(tabela, layout);

//            foreach (var buf in lista)
//                dt.Rows.Add(buf.Values.AsSpan(0, buf.FieldCount).ToArray());

//            var bulk = new MySqlBulkCopy(_conn)
//            {
//                DestinationTableName = tabela,
//                BulkCopyTimeout = BULK_TIMEOUT_SEC
//            };

//            for (int i = 0; i < dt.Columns.Count; i++)
//                bulk.ColumnMappings.Add(new MySqlBulkCopyColumnMapping(i, dt.Columns[i].ColumnName));

//            try
//            {
//                await bulk.WriteToServerAsync(dt, cancellationToken);
//            }
//            catch (MySqlException ex)
//            {
//                Console.WriteLine($"[MYSQL ERROR] {tabela} | {ex.Number} | {ex.SqlState} | {ex.Message}");
//                throw;
//            }

//            sw.Stop();

//            DevolverArrays(lista);
//            lista.Clear();

//            _totalLinhas += count;
//            _totalFlushes++;
//            _tempoTotalFlushMs += sw.ElapsedMilliseconds;
//        }

//        private DataTable ObterSchemaClonado(string tabela, EcfLayout layout)
//        {
//            if (!_schemaCache.TryGetValue(tabela, out var schema))
//            {
//                schema = CriarSchema(tabela, layout);
//                _schemaCache[tabela] = schema;
//            }
//            // Clone() copia so a estrutura de colunas — sem nenhuma linha
//            return schema.Clone();
//        }

//        private DataTable CriarSchema(string tabela, EcfLayout layout)
//        {
//            var campos = layout.GetCampos(_layoutNumber);
//            var dt = new DataTable(tabela);

//            dt.Columns.Add("id", typeof(long));
//            dt.Columns.Add("id_pai", typeof(long));
//            dt.Columns.Add("file_id", typeof(long));
//            dt.Columns.Add("id_op", typeof(long));

//            foreach (var campo in campos)
//            {
//                var tipo = campo.Tipo switch
//                {
//                    "decimal" => typeof(decimal),
//                    "int" => typeof(int),
//                    "long" => typeof(long),
//                    "date" => typeof(DateTime),
//                    _ => typeof(string)
//                };

//                var col = string.IsNullOrWhiteSpace(campo.Coluna) ? campo.Nome : campo.Coluna;
//                dt.Columns.Add(col, tipo);
//            }

//            return dt;
//        }

//        private static void DevolverArrays(List<ValueBuffer> lista)
//        {
//            foreach (var buf in lista)
//            {
//                var b = buf;
//                b.Return();
//            }
//        }

//        public async Task FlushAllAsync(CancellationToken cancellationToken = default)
//        {
//            foreach (var tabela in _batches.Keys.ToList())
//                await FlushAsync(tabela, cancellationToken);
//        }

//        public void ImprimirMetricas()
//        {
//            Console.WriteLine($"\n[BULK WRITER METRICAS]");
//            Console.WriteLine($"  Total inserido:    {_totalLinhas:N0} linhas");
//            Console.WriteLine($"  Total flushes:     {_totalFlushes:N0}");
//            Console.WriteLine($"  Tempo total banco: {_tempoTotalFlushMs:N0}ms");
//            if (_totalFlushes > 0)
//                Console.WriteLine($"  Media por flush:   {_tempoTotalFlushMs / _totalFlushes:N0}ms");
//        }

//        public async ValueTask DisposeAsync()
//        {
//            if (_disposed) return;
//            _disposed = true;
//            await FlushAllAsync(CancellationToken.None);
//        }
//    }
//}