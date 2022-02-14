using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lens.Core.Lib.Services
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);
        void QueueBackgroundWorkItem<TService>(Func<TService, CancellationToken, Task> workItem);
        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}
