using Domain.Entities.Sped.Ecf;
using EFCore.BulkExtensions;
using Infrastructure.Context;
using Infrastructure.Interface;

namespace Infrastructure.Repositories
{
    public class BulkInsertService : IBulkInsertService
    {
        private readonly CPGDbContext _context;

        public BulkInsertService(CPGDbContext context)
        {
            _context = context;
        }

        public async Task FlushAsync(
            Dictionary<Type, IList<EcfBase>> buffer,
            CancellationToken cancellationToken = default)
        {
            if (buffer.Count == 0)
                return;

            using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);


            foreach (var (_, list) in buffer)
            {
                if (list.Count == 0)
                    continue;

                //foreach (var item in list)
                //{
                //    Console.WriteLine($"Preparando insert: {string.Join(", ", item.GetType().GetProperties().Select(p => $"{p.Name}={p.GetValue(item)}"))}");
                //}

                //var bulkConfig = new BulkConfig
                //{
                //    BatchSize = 2000,
                //    PreserveInsertOrder = true, // garante que pais sejam inseridos antes de filhos
                //    TrackingEntities = true,       // captura melhor erros de relacionamento
                //    SetOutputIdentity = true,
                //    UseTempDB = false,             // MySQL
                //    CalculateStats = false// atualiza IDs gerados se houver identity
                //};
                var bulkConfig = new BulkConfig
                {
                    BatchSize = 5000,                 // MySQL aguenta bem 5k–10k
                    PreserveInsertOrder = true,       // mantém pai → filho
                    TrackingEntities = false,         // 🔥 grande ganho de performance
                    SetOutputIdentity = false,        // 🔥 só ligue se REALMENTE precisar do ID gerado
                    UseTempDB = false,                // correto para MySQL
                    CalculateStats = false,
                    EnableStreaming = true            // 🔥 reduz uso de memória
                };




                //await _context.BulkInsertAsync(
                //    list,
                //    new BulkConfig
                //    {
                //        BatchSize = 3000,            // MySQL-friendly
                //        PreserveInsertOrder = true,    // evita bugs com FK
                //        TrackingEntities = true,      // performance
                //        UseTempDB = false,             // MySQL
                //        CalculateStats = false

                //    },
                //    cancellationToken: cancellationToken
                //);
                //for (int i = 0; i < list.Count; i++)
                //{
                //    var item = list[i];
                //    Console.WriteLine($"[ORDEM {i}] ID:{item.Id} Chave:{item}");
                //}


                try
                {
                    await _context.BulkInsertAsync(list, bulkConfig);
                }
                catch (MySqlConnector.MySqlException ex)
                {
                    Console.WriteLine("🚨 MySQL Bulk Insert Falhou!");
                    Console.WriteLine($"Mensagem principal: {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"InnerException: {ex.InnerException.Message}");

                    // Se quiser detalhes ainda mais completos
                    Console.WriteLine($"Erro completo: {ex}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("🚨 Erro genérico no Bulk Insert:");
                    Console.WriteLine(ex);
                }
            }

            await tx.CommitAsync(cancellationToken);
        }
    }
}
