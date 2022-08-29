namespace Lens.Core.Lib.Services;

public interface IBackgroundTaskQueue
{
    void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);
    void QueueBackgroundWorkItem<TService>(Func<TService, CancellationToken, Task> workItem)
        where TService : notnull;
    Task<Func<CancellationToken, Task>?> DequeueAsync(CancellationToken cancellationToken);
}
