//using Domain.Entities.Sped;
//using Domain.Interfaces;
//using Infrastructure.Repositories.Bulk;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Hosting;
//using System.Threading.Channels;

//namespace Infrastructure.BackgroundJobs;

//public sealed class BulkWorker : BackgroundService
//{
//    private readonly ChannelReader<ParsedRow> _channel;
//    private readonly string _connectionString;
//    private readonly ILayoutRepository _layoutRepository;

//    public BulkWorker(
//        ChannelReader<ParsedRow> channel,
//        IConfiguration configuration , ILayoutRepository layoutRepository )
//    {
//        _channel = channel;

//        _connectionString =
//            configuration.GetConnectionString("DefaultConnection")
//            ?? throw new InvalidOperationException("Connection string não configurada.");
//        _layoutRepository = layoutRepository;
//    }

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        try
//        {
//            Console.WriteLine("BulkWorker iniciado.");

//            var consumer = new BulkConsumer(
//                _channel,
//                _connectionString, _layoutRepository);

//            await consumer.StartAsync(stoppingToken);

//            Console.WriteLine("BulkWorker finalizado normalmente.");
//        }
//        catch (OperationCanceledException)
//        {
//            Console.WriteLine("BulkWorker cancelado.");
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"ERRO NO BULK WORKER:{ex}");
//            Console.WriteLine(ex.ToString());
//        }
//    }
//}