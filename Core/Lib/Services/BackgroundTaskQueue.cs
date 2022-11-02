using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Lens.Core.Lib.Services;

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly ConcurrentQueue<Func<CancellationToken, Task>> _workItems = new();
    private readonly SemaphoreSlim _signal = new(0);
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public BackgroundTaskQueue(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
    {
        if (workItem == null)
        {
            throw new ArgumentNullException(nameof(workItem));
        }

        _workItems.Enqueue(workItem);
        _signal.Release();
    }

    public void QueueBackgroundWorkItem<TService>(Func<TService, CancellationToken, Task> workItem)
        where TService : notnull
    {
        if (workItem == null)
        {
            throw new ArgumentNullException(nameof(workItem));
        }

        _workItems.Enqueue(async token =>
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var service = (scope ?? throw new Exception("Could not create a scope.")).ServiceProvider.GetRequiredService<TService>();
            await workItem(service, token);
        });

        _signal.Release();
    }

    public async Task<Func<CancellationToken, Task>?> DequeueAsync(CancellationToken cancellationToken)
    {
        await _signal.WaitAsync(cancellationToken);
        _workItems.TryDequeue(out var workItem);

        return workItem;
    }
}
