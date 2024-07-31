using System.Collections.Concurrent;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.Async;

internal class AsyncScope
{
    private readonly ConcurrentDictionary<Task<AzothValue>, Void> tasks = new();

    public void Add(Task<AzothValue> task)
    {
        if (task.IsCompleted) return;

        tasks.TryAdd(task, default);

        // Attach a continuation to remove the task from the collection when it completes. Do so
        // after adding it to the collection so that if it completed before we added it, it will be
        // removed immediately.
        task.ContinueWith(t => tasks.TryRemove(t, out _), TaskContinuationOptions.ExecuteSynchronously);
    }

    public async ValueTask ExitAsync() => await Task.WhenAll(tasks.Keys);
}
