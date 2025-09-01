using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;
using Void = Azoth.Tools.Bootstrap.Framework.Void;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.Async;

internal sealed class AsyncScope
{
    private readonly ConcurrentDictionary<Task<Result>, Void> tasks = new();

    public void Add(Task<Result> task)
    {
        if (task.IsCompleted) return;

        tasks.TryAdd(task, default);

        // Attach a continuation to remove the task from the collection when it completes. Do so
        // after adding it to the collection so that if it completed before we added it, it will be
        // removed immediately.
        task.ContinueWith(t => tasks.TryRemove(t, out _), TaskContinuationOptions.ExecuteSynchronously);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task ExitAsync() => Task.WhenAll(tasks.Keys);
}
