using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Azoth.Tools.Bootstrap.Framework.Collections;

public sealed class PriorityQueue<T>
{
    private readonly PriorityQueue<T, T> queue;

    public PriorityQueue()
    {
        queue = new();
    }

    public PriorityQueue(IComparer<T> comparer)
    {
        queue = new(comparer);
    }

    public void Enqueue(T value) => queue.Enqueue(value, value);

    public bool TryDequeue([MaybeNullWhen(false)] out T value)
        => queue.TryDequeue(out value, out _);
}
