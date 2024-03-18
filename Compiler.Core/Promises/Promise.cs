using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Promises;

/// <summary>
/// A simple promise of a future value. The value can be set only once.
/// </summary>
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class Promise<T> : IPromise<T>
{
    internal static Promise<T> Default { get; } = new();

    public bool IsFulfilled { get; private set; }
    private T value = default!;

    [DebuggerHidden]
    public Promise() { }

    [DebuggerHidden]
    public Promise(T value)
    {
        this.value = value;
        IsFulfilled = true;
    }

    [DebuggerHidden]
    public T Fulfill(T value)
    {
        Requires.That(nameof(IsFulfilled), !IsFulfilled, "must not already be fulfilled");
        this.value = value;
        IsFulfilled = true;
        return value;
    }

    [DebuggerHidden]
    public T Result
    {
        get
        {
            if (!IsFulfilled) throw new InvalidOperationException("Promise not fulfilled");

            return value;
        }
    }

    // Useful for debugging
    public override string ToString()
        => IsFulfilled ? value?.ToString() ?? Promise.NullString : Promise.PendingString;
}

public static class Promise
{
    public static Promise<T> ForValue<T>(T value) => new(value);

    public static Promise<T?> Null<T>()
        where T : class
        => Promise<T?>.Default;

    public const string PendingString = "⧼pending⧽";
    public const string NullString = "⧼null⧽";
}
