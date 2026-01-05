using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Application.teste
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<Func<IServiceProvider, CancellationToken, Task>> _queue =
            Channel.CreateUnbounded<Func<IServiceProvider, CancellationToken, Task>>();

        public void Enqueue(Func<IServiceProvider, CancellationToken, Task> workItem)
        {
            _queue.Writer.TryWrite(workItem);
            Console.WriteLine($"[Fila] Item enfileirado. Total: {_queue.Reader.Count}");
        }

        public async IAsyncEnumerable<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            while (await _queue.Reader.WaitToReadAsync(cancellationToken))
            {
                if (_queue.Reader.TryRead(out var workItem))
                    yield return workItem;
            }
        }
    }

}
