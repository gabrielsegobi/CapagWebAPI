using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Application.teste
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IServiceScopeFactory _scopeFactory;

        public QueuedHostedService(IBackgroundTaskQueue taskQueue, IServiceScopeFactory scopeFactory)
        {
            _taskQueue = taskQueue;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var workItem in _taskQueue.DequeueAsync(stoppingToken))
            {
                using var scope = _scopeFactory.CreateScope();

                try
                {
                    await workItem(scope.ServiceProvider, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[HostedService] Erro no processamento: {ex.Message}");
                }
            }
        }
    }
}
