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

                try
                {
                    await _context.BulkInsertAsync(list, bulkConfig);
                }
                catch (MySqlConnector.MySqlException ex)
                {
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
