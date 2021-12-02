using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static System.Threading.Timeout;
using Debug = System.Diagnostics.Debug;

namespace DotNext.Threading.Tasks;

/// <summary>
/// Represents a pipe to process asynchronous tasks as they complete.
/// </summary>
/// <typeparam name="T">The type of the task.</typeparam>
public partial class TaskCompletionPipe<T> : IAsyncEnumerable<T>
    where T : Task
{
    // Represents a number of scheduled tasks which can be greater than the number of enqueued tasks
    // because only completed task can be enqueued
    private uint scheduledTasksCount;
    private bool completionRequested;

    /// <summary>
    /// Initializes a new pipe.
    /// </summary>
    /// <param name="capacity">The expected number of tasks to be placed to the pipe.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
    public TaskCompletionPipe(int capacity = 0)
    {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        pool = new(OnCompleted);
        completedTasks = new(capacity);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private void OnCompleted(Signal signal)
    {
        if (ReferenceEquals(this.signal, signal))
            this.signal = null;

        pool.Return(signal);
    }

    /// <summary>
    /// Marks the pipe as being complete, meaning no more items will be added to it.
    /// </summary>
    /// <exception cref="InvalidOperationException">The pipe is already completed.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Complete()
    {
        if (completionRequested)
            throw new InvalidOperationException();

        if (scheduledTasksCount == 0 && (signal?.TrySetResult(false) ?? false))
            signal = null;

        completionRequested = true;
    }

    private void Notify()
    {
        Debug.Assert(Monitor.IsEntered(this));

        if (signal?.TrySetResult(true) ?? false)
            signal = null;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private bool TryAdd(T task)
    {
        if (completionRequested)
            throw new InvalidOperationException();

        scheduledTasksCount++;

        if (task.IsCompleted)
        {
            completedTasks.Enqueue(task);
            Notify();
            return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private void AddSynchronized(T task)
    {
        completedTasks.Enqueue(task);
        Notify();
    }

    /// <summary>
    /// Adds the task to this pipe.
    /// </summary>
    /// <param name="task">The task to add.</param>
    /// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The pipe is closed.</exception>
    public void Add(T task)
    {
        ArgumentNullException.ThrowIfNull(task);

        if (!TryAdd(task))
            task.ConfigureAwait(false).GetAwaiter().OnCompleted(() => AddSynchronized(task));
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private bool TryDequeue([MaybeNullWhen(false)]out T task) => completedTasks.TryDequeue(out task);

    [MethodImpl(MethodImplOptions.Synchronized)]
    private ValueTask<bool> TryDequeue(out T? task, CancellationToken token)
    {
        ValueTask<bool> result;

        if (completedTasks.TryDequeue(out task))
        {
            scheduledTasksCount--;
            result = new(true);
        }
        else if (scheduledTasksCount == 0 && completionRequested)
        {
            result = new(false);
        }
        else
        {
            var source = pool.Get();
            signal = source;
            result = source.CreateTask(InfiniteTimeSpan, token);
        }

        return result;
    }

    /// <summary>
    /// Attempts to read the completed task synchronously.
    /// </summary>
    /// <param name="task">The completed task.</param>
    /// <returns><see langword="true"/> if a task was read; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool TryRead([MaybeNullWhen(false)]out T task)
    {
        if (completedTasks.TryDequeue(out task))
        {
            scheduledTasksCount--;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Waits for the first completed task.
    /// </summary>
    /// <param name="token">The token that can be used to cancel the operation.</param>
    /// <returns><see langword="true"/> if data is available to read; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public ValueTask<bool> WaitToReadAsync(CancellationToken token = default)
    {
        ValueTask<bool> result;
        if (!completedTasks.IsEmpty)
        {
            result = new(true);
        }
        else if (scheduledTasksCount == 0 && completionRequested)
        {
            result = new(false);
        }
        else
        {
            var source = pool.Get();
            signal = source;
            result = source.CreateTask(InfiniteTimeSpan, token);
        }

        return result;
    }

    /// <summary>
    /// Gets the enumerator to get the completed tasks.
    /// </summary>
    /// <param name="token">The token that can be used to cancel the operation.</param>
    /// <returns>The enumerator over completed tasks.</returns>
    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken token)
    {
        while (await TryDequeue(out var task, token).ConfigureAwait(false))
        {
            if (task is not null)
            {
                Debug.Assert(task.IsCompleted);

                yield return task;
            }
        }
    }
}