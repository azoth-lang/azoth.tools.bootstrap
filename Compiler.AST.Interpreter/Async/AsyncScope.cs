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
        task.ContinueWith(t => tasks.TryRemove(t, out _), TaskContinuationOptions.ExecuteSynchronously);
        tasks.TryAdd(task, default);
        // In case it completed before we could add it to the collection, remove it
        if (task.IsCompleted) tasks.TryRemove(task, out _);
    }

    public async ValueTask ExitAsync() => await Task.WhenAll(tasks.Keys);
}
