//using Domain.Entities.Sped;
//using Domain.Entities.Sped.Ecf;
//using Domain.Interfaces;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;
//using MySqlConnector;
//using System.Collections.Concurrent;
//using System.Diagnostics;
//using System.Threading.Channels;

//namespace Infrastructure.Repositories.Bulk
//{
//    public sealed class BulkConsumer
//    {
//        private readonly ChannelReader<ParsedRow> _reader;
//        private readonly string _connectionString;
//        private readonly ILayoutRepository _layoutRepository;
//        private readonly Dictionary<string, EcfLayout> _layoutCache = new();
//        private const int BatchSize = 1;

//        private readonly ConcurrentDictionary<string, List<ValueBuffer>> _buffers = new();

//        public BulkConsumer(ChannelReader<ParsedRow> reader, string connectionString , ILayoutRepository layoutRepository)
//        {
//            _reader = reader;
//            _connectionString = connectionString;
//            _layoutRepository = layoutRepository;
//        }

//        public async Task StartAsync(CancellationToken ct)
//        {
//            Console.WriteLine("🚀 BulkConsumer iniciado");
//            var layouts = await _layoutRepository.LoadAllAsync();

//            foreach (var layout in layouts)
//            {
//                _layoutCache[layout.Registro] = layout;
//            }
//            var totalProcessed = 0;
//            var swTotal = Stopwatch.StartNew();

//            using var connection = new MySqlConnection(_connectionString);
//            await connection.OpenAsync(ct);

//            Console.WriteLine("✅ Conexão com banco aberta");
//            var teste = GetDynamicColumns("0000", 1);
//            try
//            {
//                await foreach (var row in _reader.ReadAllAsync(ct))
//                {
//                    if(row.Tabela == "E_0000")
//                    {
//                        //Console.WriteLine("teste");
//                        //Console.WriteLine(row);
//                        //Console.WriteLine(row.Tabela);
//                        var buffer = row.Buffer;

//                        Console.WriteLine("===== BUFFER =====");

//                        Console.WriteLine($"Id = {buffer.Id}");
//                        Console.WriteLine($"IdPai = {buffer.IdPai}");
//                        Console.WriteLine($"FileId = {buffer.FileId}");
//                        Console.WriteLine($"IdOp = {buffer.IdOp}");
//                        //Console.WriteLine($"Competencia = {buffer.Competencia}");

//                        Console.WriteLine("---- Campos Dinâmicos ----");

//                        for (int i = 0; i < buffer.FieldCount; i++)
//                        {
//                            Console.WriteLine($"Field[{i}] = {buffer.GetField(i)}");
//                        }

//                        Console.WriteLine("==========================");
//                    }
                  
//                    if (row.Tabela != "E_0000")
//                        continue;

//                    var list = _buffers.GetOrAdd(row.Tabela, _ => new List<ValueBuffer>(BatchSize));

//                    list.Add(row.Buffer);
//                    totalProcessed++;

//                    if (totalProcessed % 10000 == 0)
//                    {
//                        Console.WriteLine($"📊 Processados: {totalProcessed:N0}");
//                    }

//                    if (list.Count >= BatchSize)
//                    {
//                        Console.WriteLine($"📦 Enviando batch de {list.Count} registros para {row.Tabela}");
//                        await FlushSafeAsync(row.Tabela, list, teste, connection, ct);
//                        list.Clear();
//                    }
//                }

//                Console.WriteLine("📥 Leitura do Channel finalizada");

//                // Flush final
//                foreach (var kvp in _buffers)
//                {
//                    if (kvp.Value.Count > 0)
//                    {
//                        Console.WriteLine($"📦 Flush final de {kvp.Value.Count} registros para {kvp.Key}");
//                        await FlushSafeAsync(kvp.Key, kvp.Value, teste, connection, ct);
//                        kvp.Value.Clear();
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("❌ ERRO CRÍTICO no processamento:");
//                Console.WriteLine(ex);
//            }
//            finally
//            {
//                swTotal.Stop();
//                Console.WriteLine($"🏁 Finalizado. Total processado: {totalProcessed:N0}");
//                Console.WriteLine($"⏱ Tempo total: {swTotal.Elapsed}");
//            }
//        }

//        private static async Task FlushSafeAsync(
//            string tabela,
//            List<ValueBuffer> buffers,
//             string[] dynamicColumns,
//            MySqlConnection connection,
//            CancellationToken ct)
//        {
//            var sw = Stopwatch.StartNew();

//            try
//            {
//                var buffer = buffers[0];

//                Console.WriteLine("===== BUFFER FIXO =====");
//                Console.WriteLine($"id = {buffer.Id}");
//                Console.WriteLine($"id_pai = {buffer.IdPai}");
//                Console.WriteLine($"file_id = {buffer.FileId}");
//                Console.WriteLine($"id_op = {buffer.IdOp}");

//                Console.WriteLine("===== BUFFER DINÂMICO =====");

//                for (int i = 0; i < buffer.FieldCount; i++)
//                {
//                    Console.WriteLine($"Field[{i}] = {buffer.GetField(i) ?? "NULL"}");
//                }
//                using var reader = new ValueBufferDataReader(buffers, dynamicColumns);

//                var bulk = new MySqlBulkCopy(connection)
//                {
//                    DestinationTableName = tabela,
//                    BulkCopyTimeout = 0
//                };

//                for (int i = 0; i < reader.FieldCount; i++)
//                {
//                    var columnName = reader.GetName(i);

//                    bulk.ColumnMappings.Add(
//                        new MySqlBulkCopyColumnMapping(i, columnName)
//                    );
//                }
//                Console.WriteLine("===== LINHA ENVIADA AO BANCO =====");

//                //for (int i = 0; i < reader.FieldCount; i++)
//                //{
//                //    var columnName = reader.GetName(i);
//                //    var value = reader.GetValue(i);

//                //    Console.WriteLine($"{columnName} = {(value == DBNull.Value ? "NULL" : value)}");
//                //}

//                Console.WriteLine("===================================");
//                await bulk.WriteToServerAsync(reader, ct);

//                sw.Stop();
//                Console.WriteLine($"✅ Batch enviado para {tabela} em {sw.ElapsedMilliseconds} ms");
//            }
//            catch (Exception ex)
//            {
//                sw.Stop();
//                Console.WriteLine($"❌ ERRO ao enviar batch para {tabela}");
//                Console.WriteLine($"⏱ Tempo até erro: {sw.ElapsedMilliseconds} ms");
//                Console.WriteLine(ex);
//            }
//        }


//        private string[] GetDynamicColumns(string registro, int numeroLayout)
//        {
//            if (!_layoutCache.TryGetValue(registro, out var layout))
//                throw new Exception($"Layout não encontrado para {registro}");

//            var campos = layout.GetCampos(numeroLayout);

//            return campos
//                .Select(c => c.Coluna) // <-- IMPORTANTE: nome da coluna no banco
//                .ToArray();
//        }
//    }


//}