//using Domain.Entities.Sped;
//using Microsoft.Extensions.Hosting;
//using System.Diagnostics;
//using System.Threading.Channels;

//namespace Infrastructure.Repositories.Bulk
//{
//    public class ParsedRowLoggerService : BackgroundService
//    {
//        private readonly ChannelReader<ParsedRow> _reader;

//        private const int BatchSize = 5000;

//        public ParsedRowLoggerService(ChannelReader<ParsedRow> reader)
//        {
//            _reader = reader;
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            Console.WriteLine("Bulk Simulator iniciado...");

//            var batch = new List<ParsedRow>(BatchSize);
//            var totalProcessado = 0;
//            var totalBatches = 0;

//            var swGlobal = Stopwatch.StartNew();

//            await foreach (var row in _reader.ReadAllAsync(stoppingToken))
//            {
//                batch.Add(row);

//                if (batch.Count >= BatchSize)
//                {
//                    await SimularBanco(batch);

//                    totalProcessado += batch.Count;
//                    totalBatches++;

//                    Console.WriteLine(
//                        $"Batch {totalBatches:N0} | " +
//                        $"Registros: {BatchSize:N0} | " +
//                        $"Total acumulado: {totalProcessado:N0}"
//                    );

//                    batch.Clear();
//                }
//            }

//            // Processa resto final
//            if (batch.Count > 0)
//            {
//                await SimularBanco(batch);
//                totalProcessado += batch.Count;
//                totalBatches++;
//            }

//            swGlobal.Stop();

//            Console.WriteLine("\n=== RESUMO FINAL ===");
//            Console.WriteLine($"Total registros: {totalProcessado:N0}");
//            Console.WriteLine($"Total batches: {totalBatches:N0}");
//            Console.WriteLine($"Tempo total: {swGlobal.Elapsed.TotalSeconds:N2}s");

//            if (swGlobal.Elapsed.TotalSeconds > 0)
//            {
//                var throughput = totalProcessado / swGlobal.Elapsed.TotalSeconds;
//                Console.WriteLine($"Throughput: {throughput:N0} registros/seg");
//            }
//        }

//        private async Task SimularBanco(List<ParsedRow> batch)
//        {
//            // 🔥 Simulação de custo de banco
//            // Imagine aqui um SqlBulkCopy real

//            await Task.Delay(10); // simula latência
//        }
//    }
//}