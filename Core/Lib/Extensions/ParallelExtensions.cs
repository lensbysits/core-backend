using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;

namespace Lens.Core.Lib.Extensions;

/// <summary>
/// See: https://medium.com/@alex.puiu/parallel-foreach-async-in-c-36756f8ebe62
/// </summary>
public static class ParallelExtensions
{
    public static Task ForEachAsync<T>(this IEnumerable<T> source, int nrOfBatches, Func<T, Task> body)
    {
        return Task.WhenAll(
            from partition in Partitioner.Create(source).GetPartitions(nrOfBatches)
            select Task.Run(async delegate
            {
                using (partition)
                    while (partition.MoveNext())
                        await body(partition.Current);
            }));
    }

    public static Task ParallelForEachAsync<T>(this IEnumerable<T> source, int nrOfBatches, Func<T, Task> body)
    {
        async Task AwaitPartition(IEnumerator<T> partition)
        {
            using (partition)
            {
                while (partition.MoveNext())
                {
                    await body(partition.Current);
                }
            }
        }

        return Task.WhenAll(
            Partitioner
                .Create(source)
                .GetPartitions(nrOfBatches)
                .AsParallel()
                .Select(AwaitPartition));
    }

    public static Task AsyncParallelForEach<T>(this IEnumerable<T> source, Func<T, Task> body, int maxDegreeOfParallelism = DataflowBlockOptions.Unbounded,
        TaskScheduler? scheduler = null)
    {
        var options = new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism
        };
        if (scheduler != null)
            options.TaskScheduler = scheduler;

        var block = new ActionBlock<T>(body, options);

        foreach (var item in source)
            block.Post(item);

        block.Complete();
        return block.Completion;
    }

#pragma warning disable AsyncFixer01 // Unnecessary async/await usage
    public static async Task AsyncParallelForEach<T>(this IAsyncEnumerable<T> source, Func<T, Task> body, int maxDegreeOfParallelism = DataflowBlockOptions.Unbounded,
        TaskScheduler? scheduler = null)
    {
        var options = new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism
        };
        if (scheduler != null)
            options.TaskScheduler = scheduler;

        var block = new ActionBlock<T>(body, options);

        await foreach (var item in source)
            block.Post(item);

        block.Complete();
        await block.Completion;
    }
#pragma warning restore AsyncFixer01 // Unnecessary async/await usage

}
