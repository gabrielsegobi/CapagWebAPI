//using Domain.Entities.Sped;
//using Domain.Entities.Sped.Ecf;
//using System;
//using System.Buffers;
//using System.Diagnostics;
//using System.IO.Pipelines;
//using System.Text;
//using System.Threading.Channels;

//namespace Infrastructure.Services
//{
//    public class SpedStreamParser
//    {
//        private readonly Dictionary<string, EcfLayout> _layouts;
//        private readonly ChannelWriter<ParsedRow> _channel;


//        private readonly Stack<(string Registro, long Id)> _ctx = new(capacity: 32);


//        private long _sequence = 0;

//        private readonly Dictionary<string, int> _contadorPorRegistro = new();
//        private int _totalLinhas = 0;
//        private int _linhasIgnoradas = 0;

//        public SpedStreamParser(
//            Dictionary<string, EcfLayout> layouts,
//            ChannelWriter<ParsedRow> channel)
//        {
//            _layouts = layouts;
//            _channel = channel;
//        }


//        public async Task ParseAsync(
//            Stream arquivo,
//            long fileId,
//            long idOp,
//            long idTenant,
//            long idEmpresa,
//            int layout,
//            string competencia)
//        {
//            var swTotal = Stopwatch.StartNew();
//            _ctx.Clear();
//            _sequence = 0;
//            _contadorPorRegistro.Clear();
//            _totalLinhas = 0;
//            _linhasIgnoradas = 0;




//            var pipe = PipeReader.Create(arquivo, new StreamPipeReaderOptions(
//                bufferSize: 65536,    
//                minimumReadSize: 4096));   

//            while (true)
//            {
//                swTotal.Restart();
//                var result = await pipe.ReadAsync();
//                var buffer = result.Buffer;

//                ProcessBuffer(ref buffer, fileId, idOp, idTenant, idEmpresa, competencia,  layout);
//                //ImprimirMetricas();
//                pipe.AdvanceTo(buffer.Start, buffer.End);

//                if (result.IsCompleted) break;
//            }
//            //ImprimirMetricas();
//            swTotal.Stop();
//            Console.WriteLine($"Tempo TOTAL ParseAsync: {swTotal.ElapsedMilliseconds} ms");
//            await pipe.CompleteAsync();


//            // NÃO chama _channel.Complete() aqui
//            // O orquestrador (SpedImportService) fecha o canal
//            // após TODOS os parsers terminarem (suporte a múltiplos arquivos)
//        }

//        private void ProcessBuffer(
//            ref ReadOnlySequence<byte> buffer,
//            long fileId, long idOp, long idTenant, long idEmpresa, string competencia, int layout)
//        {
//            var reader = new SequenceReader<byte>(buffer);

//            while (reader.TryReadTo(out ReadOnlySpan<byte> lineBytes, (byte)'\n'))
//            {
//                if (lineBytes.Length > 0 && lineBytes[^1] == '\r')
//                    lineBytes = lineBytes[..^1];

//                if (lineBytes.Length < 3) continue;

//                _totalLinhas++;
//                ProcessLine(lineBytes, fileId, idOp, idTenant, idEmpresa, layout);
//            }

//            buffer = buffer.Slice(reader.Position);
//        }


//        private void ProcessLine(
//            ReadOnlySpan<byte> lineBytes,
//            long fileId, long idOp, long idTenant, long idEmpresa, int layoutid)
//        {

//            Span<char> lineChars = lineBytes.Length <= 4096
//                ? stackalloc char[lineBytes.Length]
//                : new char[lineBytes.Length];

//            int charCount = Encoding.UTF8.GetChars(lineBytes, lineChars);
//            var line = lineChars[..charCount];


//            var codigoSpan = GetField(line, 1);
//            if (codigoSpan.IsEmpty)
//            {
//                _linhasIgnoradas++;
//                return;
//            }


//            var codigoStr = codigoSpan.ToString();
//            if (!_layouts.TryGetValue(codigoStr, out var layout))
//            {
//                _linhasIgnoradas++;
//                return;
//            }

//            var camposLayout = layout.GetCampos(layoutid);
//            int expectedCount = camposLayout.Count;

//            //// Se o último campo esperado não existir → tem menos campos
//            //if (GetField(line, expectedCount).IsEmpty)
//            //{
//            //    throw new InvalidOperationException(
//            //        $"Registro {codigoStr} com MENOS campos que o esperado ({expectedCount})."
//            //    );
//            //}

//            //// Se existir um campo depois do esperado → tem mais campos
//            //if (!GetField(line, expectedCount + 1).IsEmpty)
//            //{
//            //    throw new InvalidOperationException(
//            //        $"Registro {codigoStr} com MAIS campos que o esperado ({expectedCount})."
//            //    );
//            //}


//            AjustarContexto(layout);


//            long idAtual = ++_sequence;

//            long? idPai = null;
//            if (layout.Pai != null && _ctx.Count > 0)
//                idPai = _ctx.Peek().Id;


//            _ctx.Push((codigoStr, idAtual));


//            if (!_contadorPorRegistro.TryGetValue(codigoStr, out _))
//                _contadorPorRegistro[codigoStr] = 0;
//            _contadorPorRegistro[codigoStr]++;


//            var valueBuffer = new ValueBuffer(layout.GetCampos(layoutid).Count);

//            valueBuffer.Id = idAtual;
//            valueBuffer.IdPai = idPai;
//            valueBuffer.FileId = fileId;
//            valueBuffer.IdOp = idOp;

//            //valueBuffer.SetField(0, idAtual);
//            //valueBuffer.SetField(1, idPai);
//            //valueBuffer.SetField(2, fileId);
//            //valueBuffer.SetField(3, idOp);
//            ////valueBuffer.Set(4, idTenant);
//            ////valueBuffer.Set(5, idEmpresa);
//            //valueBuffer.Set(6, competencia);

//            for (int i = 0; i < layout.GetCampos(layoutid).Count; i++)
//            {
//                var campo = layout.GetCampos(layoutid)[i];
//                var valor = GetField(line, campo.Indice);
//                valueBuffer.SetField(i , valor.ToString());
//            }

//            if (!_channel.TryWrite(new ParsedRow(layout.Tabela, valueBuffer)))
//            {

//                _channel.WriteAsync(new ParsedRow(layout.Tabela, valueBuffer))
//                    .AsTask().GetAwaiter().GetResult();
//            }

//        }


//        private void AjustarContexto(EcfLayout layout)
//        {
//            while (_ctx.Count > 0)
//            {
//                var (registroTopo, _) = _ctx.Peek();

//                if (!_layouts.TryGetValue(registroTopo, out var layoutTopo))
//                {
//                    // Registro no topo não tem layout — descarta
//                    _ctx.Pop();
//                    continue;
//                }

//                if (layoutTopo.Nivel >= layout.Nivel)
//                    _ctx.Pop();   // descarta irmão ou filho de ciclo anterior
//                else
//                    break;        // encontrou o pai (nível menor) — para aqui
//            }
//        }

//        private static ReadOnlySpan<char> GetField(ReadOnlySpan<char> line, int indice)
//        {
//            int count = 0;
//            int start = 0;

//            for (int i = 0; i <= line.Length; i++)
//            {
//                if (i == line.Length || line[i] == '|')
//                {
//                    if (count == indice)
//                        return line.Slice(start, i - start);
//                    count++;
//                    start = i + 1;
//                }
//            }

//            return ReadOnlySpan<char>.Empty;
//        }



//        public IReadOnlyDictionary<string, int> ContadorPorRegistro => _contadorPorRegistro;
//        public int TotalLinhas => _totalLinhas;
//        public int LinhasIgnoradas => _linhasIgnoradas;
//        private Stopwatch _swTotal = new();
//        public void ImprimirMetricas()
//        {
//            Console.WriteLine($"\n[PARSER MÉTRICAS]");
//            Console.WriteLine($"  Total de linhas lidas:    {_totalLinhas:N0}");
//            Console.WriteLine($"  Linhas ignoradas:         {_linhasIgnoradas:N0}");
//            Console.WriteLine($"  Linhas processadas:       {_sequence:N0}");

//            Console.WriteLine($"\n  Por registro:");
//            foreach (var (reg, count) in _contadorPorRegistro.OrderBy(x => x.Key))
//                Console.WriteLine($"    {reg,-10} {count,10:N0} linhas");

//            Console.WriteLine($"\n  TEMPO TOTAL: {_swTotal.Elapsed.TotalSeconds:N2} s");

//            if (_sequence > 0)
//            {
//                var linhasPorSegundo = _sequence / _swTotal.Elapsed.TotalSeconds;
//                Console.WriteLine($"  LINHAS / SEGUNDO: {linhasPorSegundo:N0}");
//            }
//        }
//    }
//}


//using Domain.Entities;
//using Domain.Entities.Sped;
//using System.Buffers;
//using System.Globalization;
//using System.IO.Pipelines;
//using System.Text;
//using System.Threading.Channels;

//namespace Infrastructure.Parsers.Sped
//{
//    /// <summary>
//    /// Etapa 6: SpedStreamParser com otimizações finais.
//    ///
//    /// O que mudou em relação à Etapa 2/3:
//    ///   1. ValueBuffer.Rent() — array do pool, zero GC
//    ///   2. _layoutsIndexados  — índice por primeiro char, busca mais rápida
//    ///   3. Encoding ISO-8859-1 configurável — seu arquivo usa Latin1
//    ///   4. stackalloc com fallback — linhas > 4096 chars não crasham
//    ///   5. WriteAsync com await real — sem .GetAwaiter().GetResult()
//    /// </summary>
//    public class SpedStreamParserV2
//    {
//        // ── dependências ─────────────────────────────────────────────
//        private readonly Dictionary<string, RegistroLayout> _layouts;
//        private readonly ChannelWriter<ParsedRow> _channel;

//        // Etapa 6: índice por primeiro char do código
//        // "P100", "P030" → grupo 'P'
//        // "L001", "L030" → grupo 'L'
//        // Reduz espaço de busca de 200 para ~10-20 por grupo
//        private readonly Dictionary<char, Dictionary<string, RegistroLayout>> _layoutsIndexados;

//        // ── estado por arquivo ────────────────────────────────────────
//        private readonly Stack<(string Registro, long Id)> _ctx = new(capacity: 32);
//        private long _sequence = 0;

//        // ── encoding ─────────────────────────────────────────────────
//        // Seu arquivo ECF usa ISO-8859-1 (Latin1) — não UTF-8
//        private static readonly Encoding _encoding = Encoding.GetEncoding("ISO-8859-1");

//        // ── métricas ─────────────────────────────────────────────────
//        private readonly Dictionary<string, int> _contadorPorRegistro = new();
//        private int _totalLinhas;
//        private int _linhasIgnoradas;

//        public SpedStreamParserV2(
//            Dictionary<string, RegistroLayout> layouts,
//            ChannelWriter<ParsedRow> channel)
//        {
//            _layouts = layouts;
//            _channel = channel;

//            // Etapa 6: constrói índice por primeiro char uma única vez
//            _layoutsIndexados = layouts.Values
//                .Where(l => !string.IsNullOrEmpty(l.Registro))
//                .GroupBy(l => char.ToUpper(l.Registro[0]))
//                .ToDictionary(
//                    g => g.Key,
//                    g => g.ToDictionary(l => l.Registro, StringComparer.OrdinalIgnoreCase));
//        }

//        // ── ponto de entrada ─────────────────────────────────────────
//        public async Task ParseAsync(
//            Stream stream,
//            long fileId,
//            long idOp,
//            long idTenant,
//            long idEmpresa,
//            string competencia)
//        {
//            _ctx.Clear();
//            _sequence = 0;
//            _contadorPorRegistro.Clear();
//            _totalLinhas = 0;
//            _linhasIgnoradas = 0;

//            var pipe = PipeReader.Create(stream, new StreamPipeReaderOptions(
//                bufferSize: 131_072,  // Etapa 6: 128KB — melhor para arquivos grandes
//                minimumReadSize: 4_096));

//            while (true)
//            {
//                var result = await pipe.ReadAsync();
//                var buffer = result.Buffer;

//                ProcessBuffer(ref buffer, fileId, idOp, idTenant, idEmpresa, competencia);

//                pipe.AdvanceTo(buffer.Start, buffer.End);

//                if (result.IsCompleted) break;
//            }

//            await pipe.CompleteAsync();
//        }

//        // ── processa chunk ────────────────────────────────────────────
//        private void ProcessBuffer(
//            ref ReadOnlySequence<byte> buffer,
//            long fileId, long idOp, long idTenant, long idEmpresa, string competencia)
//        {
//            var reader = new SequenceReader<byte>(buffer);

//            while (reader.TryReadTo(out ReadOnlySpan<byte> lineBytes, (byte)'\n'))
//            {
//                if (lineBytes.Length > 0 && lineBytes[^1] == '\r')
//                    lineBytes = lineBytes[..^1];

//                if (lineBytes.Length < 3) continue;

//                _totalLinhas++;
//                ProcessLine(lineBytes, fileId, idOp, idTenant, idEmpresa, competencia);
//            }

//            buffer = buffer.Slice(reader.Position);
//        }

//        // ── processa 1 linha ─────────────────────────────────────────
//        private void ProcessLine(
//            ReadOnlySpan<byte> lineBytes,
//            long fileId, long idOp, long idTenant, long idEmpresa, string competencia)
//        {
//            // Etapa 6: stackalloc com fallback para linhas longas
//            Span<char> lineChars = lineBytes.Length <= 4096
//                ? stackalloc char[lineBytes.Length]
//                : new char[lineBytes.Length];

//            int charCount = _encoding.GetChars(lineBytes, lineChars);
//            var line = lineChars[..charCount];

//            var codigoSpan = GetField(line, 1);
//            if (codigoSpan.IsEmpty) { _linhasIgnoradas++; return; }

//            // Etapa 6: lookup com índice por primeiro char
//            if (!TryGetLayout(codigoSpan, out var layout))
//            { _linhasIgnoradas++; return; }

//            AjustarContexto(layout);

//            long idAtual = ++_sequence;
//            long? idPai = layout.Pai != null && _ctx.Count > 0
//                            ? _ctx.Peek().Id
//                            : null;

//            _ctx.Push((layout.Registro, idAtual));

//            // Etapa 6: Rent() em vez de new — array do pool
//            var valueBuffer = ValueBuffer.Rent(layout.Campos.Count + ValueBuffer.COLUNAS_FIXAS);

//            valueBuffer.Set(0, idAtual);
//            valueBuffer.Set(1, idPai);
//            valueBuffer.Set(2, fileId);
//            valueBuffer.Set(3, idOp);
//            valueBuffer.Set(4, idTenant);
//            valueBuffer.Set(5, idEmpresa);
//            valueBuffer.Set(6, competencia);

//            for (int i = 0; i < layout.Campos.Count; i++)
//            {
//                var campo = layout.Campos[i];
//                var valor = GetField(line, campo.Indice);
//                valueBuffer.Set(i + ValueBuffer.COLUNAS_FIXAS, ConvertValue(valor, campo));
//            }

//            // Métricas
//            _contadorPorRegistro.TryGetValue(layout.Registro, out var count);
//            _contadorPorRegistro[layout.Registro] = count + 1;

//            // Etapa 5: WriteAsync real — sem .GetAwaiter().GetResult()
//            var row = new ParsedRow(layout.Tabela, valueBuffer);
//            if (!_channel.TryWrite(row))
//            {
//                // Canal cheio — WriteAsync de forma fire-and-forget segura
//                // Em produção com FullMode.Wait isso raramente acontece
//                _channel.WriteAsync(row).AsTask().GetAwaiter().GetResult();
//            }
//        }

//        // ── Etapa 6: lookup com índice por primeiro char ──────────────
//        private bool TryGetLayout(ReadOnlySpan<char> codigoSpan, out RegistroLayout layout)
//        {
//            layout = null!;

//            if (codigoSpan.IsEmpty) return false;

//            var primeiroChar = char.ToUpper(codigoSpan[0]);

//            if (!_layoutsIndexados.TryGetValue(primeiroChar, out var subDict))
//                return false;

//            // ToString aqui — única alocação por linha
//            return subDict.TryGetValue(codigoSpan.ToString(), out layout!);
//        }

//        // ── ajusta pilha de hierarquia ────────────────────────────────
//        private void AjustarContexto(RegistroLayout layout)
//        {
//            while (_ctx.Count > 0)
//            {
//                var (registroTopo, _) = _ctx.Peek();

//                if (!_layouts.TryGetValue(registroTopo, out var layoutTopo))
//                { _ctx.Pop(); continue; }

//                if (layoutTopo.Nivel >= layout.Nivel)
//                    _ctx.Pop();
//                else
//                    break;
//            }
//        }

//        // ── extrai campo por índice sem Split ─────────────────────────
//        private static ReadOnlySpan<char> GetField(ReadOnlySpan<char> line, int indice)
//        {
//            int count = 0;
//            int start = 0;

//            for (int i = 0; i <= line.Length; i++)
//            {
//                if (i == line.Length || line[i] == '|')
//                {
//                    if (count == indice)
//                        return line.Slice(start, i - start);
//                    count++;
//                    start = i + 1;
//                }
//            }

//            return ReadOnlySpan<char>.Empty;
//        }

//        // ── converte span para tipo do layout ─────────────────────────
//        private static object? ConvertValue(ReadOnlySpan<char> valor, FieldLayout campo)
//        {
//            if (valor.IsEmpty || valor.IsWhiteSpace()) return null;

//            return campo.Tipo switch
//            {
//                "decimal" => decimal.TryParse(valor,
//                                 NumberStyles.Any, CultureInfo.InvariantCulture,
//                                 out var d) ? d : (object?)null,

//                "int" => int.TryParse(valor, out var i) ? (object?)i : null,
//                "long" => long.TryParse(valor, out var l) ? (object?)l : null,

//                //"date" => DateTime.TryParseExact(valor,
//                //                 campo.Formato ?? "ddMMyyyy",
//                //                 CultureInfo.InvariantCulture,
//                //                 DateTimeStyles.None, out var dt) ? dt : (object?)null,

//                _ => valor.ToString()
//            };
//        }

//        // ── métricas públicas ─────────────────────────────────────────
//        public IReadOnlyDictionary<string, int> ContadorPorRegistro => _contadorPorRegistro;
//        public int TotalLinhas => _totalLinhas;
//        public int LinhasIgnoradas => _linhasIgnoradas;

//        public void ImprimirMetricas()
//        {
//            Console.WriteLine($"\n[PARSER V2 MÉTRICAS]");
//            Console.WriteLine($"  Total linhas lidas:    {_totalLinhas:N0}");
//            Console.WriteLine($"  Linhas ignoradas:      {_linhasIgnoradas:N0}");
//            Console.WriteLine($"  Linhas processadas:    {_sequence:N0}");
//            Console.WriteLine($"\n  Por registro:");
//            foreach (var (reg, count) in _contadorPorRegistro.OrderBy(x => x.Key))
//                Console.WriteLine($"    {reg,-10} {count,10:N0} linhas");
//        }
//    }
//}


using Domain.Entities.Sped;
using Domain.Entities.Sped.Ecf;
using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Channels;

namespace Infrastructure.Parsers.Sped
{
    /// <summary>
    /// Parser de alta performance para arquivos ECF.
    ///
    /// Usa EcfLayout com versionamento — GetCampos(layoutNumber) resolve
    /// qual versão dos campos usar para o layout informado. O(1).
    ///
    /// O layoutNumber vem da linha 0000 do arquivo e é passado via ParseAsync.
    /// </summary>
    public class SpedStreamParser
    {
        // ── dependências ─────────────────────────────────────────────
        private readonly Dictionary<string, EcfLayout> _layouts;
        private readonly ChannelWriter<ParsedRow> _channel;

        // Índice por primeiro char — reduz busca de 200 → ~15 por grupo
        private readonly Dictionary<char, Dictionary<string, EcfLayout>> _layoutsIndexados;

        // ── estado por arquivo ────────────────────────────────────────
        private readonly Stack<(string Registro, long Id)> _ctx = new(capacity: 32);
        private long _sequence;

        // ── encoding ─────────────────────────────────────────────────
        // ECF usa ISO-8859-1 (Latin1)
        private static readonly Encoding _encoding = Encoding.GetEncoding("ISO-8859-1");

        // ── métricas ─────────────────────────────────────────────────
        private readonly Dictionary<string, int> _contadorPorRegistro = new();
        private int _totalLinhas;
        private int _linhasIgnoradas;

        public SpedStreamParser(
            Dictionary<string, EcfLayout> layouts,
            ChannelWriter<ParsedRow> channel)
        {
            _layouts = layouts;
            _channel = channel;

            // Constrói índice por primeiro char uma única vez
            _layoutsIndexados = layouts.Values
                .Where(l => !string.IsNullOrEmpty(l.Registro))
                .GroupBy(l => char.ToUpper(l.Registro[0]))
                .ToDictionary(
                    g => g.Key,
                    g => g.ToDictionary(l => l.Registro, StringComparer.OrdinalIgnoreCase));
        }

        // ── ponto de entrada ─────────────────────────────────────────
        /// <param name="layoutNumber">
        /// Número do layout ECF extraído da linha 0000 (campo COD_VER).
        /// Usado pelo EcfLayout.GetCampos() para selecionar a versão correta.
        /// </param>
        public async Task ParseAsync(
            Stream stream,
            long fileId,
            long idOp,
            long idTenant,
            long idEmpresa,
            int layoutNumber
            )
        {
            // Reseta estado — suporta reusar a instância para múltiplos arquivos
            _ctx.Clear();
            _sequence = 0;
            _totalLinhas = 0;
            _linhasIgnoradas = 0;
            _contadorPorRegistro.Clear();

            var inicio = DateTime.Now;
            Console.WriteLine($"\n[PARSER] Início ParseAsync  FileId={fileId}  Op={idOp}  Horário={inicio:O}");

            var swTotal = Stopwatch.StartNew();

            var pipe = PipeReader.Create(stream, new StreamPipeReaderOptions(
                bufferSize: 131_072,  // 128KB — melhor para arquivos > 1GB
                minimumReadSize: 4_096));

            while (true)
            {
                var result = await pipe.ReadAsync();
                var buffer = result.Buffer;

                ProcessBuffer(ref buffer, fileId, idOp, idTenant, idEmpresa,
                    layoutNumber);

                pipe.AdvanceTo(buffer.Start, buffer.End);

                if (result.IsCompleted) break;
            }

            await pipe.CompleteAsync();

            swTotal.Stop();
            var fim = DateTime.Now;

            var segundos = swTotal.Elapsed.TotalSeconds > 0
                ? swTotal.Elapsed.TotalSeconds
                : 0.000001; // evita divisão por zero em arquivos muito pequenos

            var linhasProcessadas = TotalProcessadas;
            var linhasPorSegundo = linhasProcessadas / segundos;

            Console.WriteLine($"\n[PARSER] Fim ParseAsync    FileId={fileId}  Op={idOp}  Horário={fim:O}");
            Console.WriteLine($"[PARSER] Duração total: {swTotal.Elapsed.TotalSeconds:N2}s ({swTotal.ElapsedMilliseconds} ms)");
            Console.WriteLine($"[PARSER] Linhas lidas: {_totalLinhas:N0}  Linhas processadas: {linhasProcessadas:N0}  Linhas ignoradas: {_linhasIgnoradas:N0}");
            Console.WriteLine($"[PARSER] Throughput aproximado: {linhasPorSegundo:N0} linhas/segundo");

            // NÃO fecha o canal aqui — o orquestrador fecha após todos os parsers
        }

        // ── processa chunk do PipeReader ──────────────────────────────
        private void ProcessBuffer(
            ref ReadOnlySequence<byte> buffer,
            long fileId, long idOp, long idTenant, long idEmpresa,
            int layoutNumber)
        {
            var reader = new SequenceReader<byte>(buffer);

            while (reader.TryReadTo(out ReadOnlySpan<byte> lineBytes, (byte)'\n'))
            {
                // Remove \r (CRLF)
                if (lineBytes.Length > 0 && lineBytes[^1] == '\r')
                    lineBytes = lineBytes[..^1];

                if (lineBytes.Length < 3) continue;

                _totalLinhas++;
                ProcessLine(lineBytes, fileId, idOp, idTenant, idEmpresa,
                    layoutNumber);
            }

            buffer = buffer.Slice(reader.Position);
        }

        // ── processa 1 linha ─────────────────────────────────────────
        private void ProcessLine(
            ReadOnlySpan<byte> lineBytes,
            long fileId, long idOp, long idTenant, long idEmpresa,
            int layoutNumber)
        {
            // Converte bytes → chars sem alocar string (stackalloc)
            Span<char> lineChars = lineBytes.Length <= 4096
                ? stackalloc char[lineBytes.Length]
                : new char[lineBytes.Length];

            int charCount = _encoding.GetChars(lineBytes, lineChars);
            var line = lineChars[..charCount];

            // Extrai código do registro (campo índice 1)
            var codigoSpan = GetField(line, 1);
            if (codigoSpan.IsEmpty) { _linhasIgnoradas++; return; }

            // Lookup com índice por primeiro char
            if (!TryGetLayout(codigoSpan, out var ecfLayout))
            { _linhasIgnoradas++; return; }

            // Resolve campos para o layoutNumber informado
            var campos = ecfLayout.GetCampos(layoutNumber);
            if (campos.Count == 0)
            {
                // Registro existe mas não tem campos para este layout — ignora
                _linhasIgnoradas++;
                return;
            }

            // Ajusta hierarquia via pilha
            AjustarContexto(ecfLayout);

            long idAtual = ++_sequence;
            long? idPai = ecfLayout.Pai != null && _ctx.Count > 0
                            ? _ctx.Peek().Id
                            : null;

            _ctx.Push((ecfLayout.Registro, idAtual));

            // Atualiza métricas
            _contadorPorRegistro.TryGetValue(ecfLayout.Registro, out var cnt);
            _contadorPorRegistro[ecfLayout.Registro] = cnt + 1;

            // Preenche ValueBuffer com ArrayPool
            var valueBuffer = ValueBuffer.Rent(campos.Count + ValueBuffer.COLUNAS_FIXAS);

            // Colunas fixas — posições 0–6
            valueBuffer.Set(0, idAtual);
            valueBuffer.Set(1, idPai);
            valueBuffer.Set(2, fileId);
            valueBuffer.Set(3, idOp);
            //valueBuffer.Set(4, idTenant);
            //valueBuffer.Set(5, idEmpresa);
            //valueBuffer.Set(6, competencia);

            // Campos do layout — posições 7+
            for (int i = 0; i < campos.Count; i++)
            {
                var campo = campos[i];
                var valor = GetField(line, campo.Indice);
                valueBuffer.Set(i + ValueBuffer.COLUNAS_FIXAS, ConvertValue(valor, campo));
            }

            // Empurra no Channel
            var row = new ParsedRow(ecfLayout.Tabela, valueBuffer);
            if (!_channel.TryWrite(row))
            {
                // Backpressure — canal cheio, aguarda espaço
                _channel.WriteAsync(row).AsTask().GetAwaiter().GetResult();
            }
        }

        // ── lookup com índice por primeiro char ───────────────────────
        private bool TryGetLayout(ReadOnlySpan<char> codigoSpan, out EcfLayout layout)
        {
            layout = null!;
            if (codigoSpan.IsEmpty) return false;

            var primeiroChar = char.ToUpper(codigoSpan[0]);
            if (!_layoutsIndexados.TryGetValue(primeiroChar, out var subDict))
                return false;

            return subDict.TryGetValue(codigoSpan.ToString(), out layout!);
        }

        // ── ajusta pilha de hierarquia ────────────────────────────────
        private void AjustarContexto(EcfLayout layout)
        {
            while (_ctx.Count > 0)
            {
                var (registroTopo, _) = _ctx.Peek();

                if (!_layouts.TryGetValue(registroTopo, out var layoutTopo))
                { _ctx.Pop(); continue; }

                if (layoutTopo.Nivel >= layout.Nivel)
                    _ctx.Pop();
                else
                    break;
            }
        }

        // ── extrai campo por índice sem Split e sem alocar ────────────
        private static ReadOnlySpan<char> GetField(ReadOnlySpan<char> line, int indice)
        {
            int count = 0;
            int start = 0;

            for (int i = 0; i <= line.Length; i++)
            {
                if (i == line.Length || line[i] == '|')
                {
                    if (count == indice)
                        return line.Slice(start, i - start);
                    count++;
                    start = i + 1;
                }
            }

            return ReadOnlySpan<char>.Empty;
        }

        // ── converte span para o tipo do campo ────────────────────────
        private static object? ConvertValue(ReadOnlySpan<char> valor, FieldLayout campo)
        {
            if (valor.IsEmpty || valor.IsWhiteSpace()) return null;

            return campo.Tipo switch
            {
                "decimal" => decimal.TryParse(valor,
                                 NumberStyles.Any, CultureInfo.InvariantCulture,
                                 out var d) ? d : (object?)null,

                "int" => int.TryParse(valor, out var i) ? (object?)i : null,
                "long" => long.TryParse(valor, out var l) ? (object?)l : null,

                //"date" => DateTime.TryParseExact(valor,
                //              campo.Formato ?? "ddMMyyyy",
                //              CultureInfo.InvariantCulture,
                //              DateTimeStyles.None, out var dt) ? dt : (object?)null,

                // "string" e qualquer outro tipo — única alocação inevitável
                _ => valor.ToString()
            };
        }

        // ── métricas públicas ─────────────────────────────────────────
        public IReadOnlyDictionary<string, int> ContadorPorRegistro => _contadorPorRegistro;
        public int TotalLinhas => _totalLinhas;
        public int LinhasIgnoradas => _linhasIgnoradas;
        public long TotalProcessadas => _sequence;

        public void ImprimirMetricas()
        {
            Console.WriteLine($"\n[PARSER V2 MÉTRICAS]");
            Console.WriteLine($"  Total linhas lidas:    {_totalLinhas:N0}");
            Console.WriteLine($"  Linhas ignoradas:      {_linhasIgnoradas:N0}");
            Console.WriteLine($"  Linhas processadas:    {_sequence:N0}");
            Console.WriteLine($"\n  Por registro:");
            foreach (var (reg, count) in _contadorPorRegistro.OrderBy(x => x.Key))
                Console.WriteLine($"    {reg,-10} {count,10:N0} linhas");
        }
    }
}