using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Promises;

/// <summary>
/// A promise that helps in detecting cycles in the computation of promised
/// values so that they can be forced to be acyclic.
/// </summary>
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public class AcyclicPromise<T> : IPromise<T>
{
    private PromiseState state;
    public bool IsFulfilled => state == PromiseState.Fulfilled;
    private T value = default!;

    public AcyclicPromise()
    {
        state = PromiseState.Pending;
    }

    public AcyclicPromise(T value)
    {
        this.value = value;
        state = PromiseState.Fulfilled;
    }

    [DebuggerHidden]
    public void BeginFulfilling()
    {
        Requires.That(nameof(state), state == PromiseState.Pending, "must be pending is " + state);
        state = PromiseState.InProgress;
    }

    /// <returns><see langword="true"/> if the caller can begin fulfilling the promise.</returns>
    [DebuggerHidden]
    public bool TryBeginFulfilling(Action? whenInProgress = null)
    {
        switch (state)
        {
            default:
                throw ExhaustiveMatch.Failed(state);
            case PromiseState.InProgress:
                whenInProgress?.Invoke();
                return false;
            case PromiseState.Fulfilled:
                // We have already resolved it
                return false;
            case PromiseState.Pending:
                state = PromiseState.InProgress;
                // we need to compute it
                return true;
        }
    }

    [DebuggerHidden]
    public T Fulfill(T value)
    {
        Requires.That(nameof(state), state == PromiseState.InProgress, "must be in progress is " + state);
        this.value = value;
        state = PromiseState.Fulfilled;
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
    {
        return state switch
        {
            PromiseState.Pending => "⧼pending⧽",
            PromiseState.InProgress => "⧼in progress⧽",
            PromiseState.Fulfilled => value?.ToString() ?? "⧼null⧽",
            _ => throw ExhaustiveMatch.Failed(state)
        };
    }
}

public static class AcyclicPromise
{
    public static AcyclicPromise<T> ForValue<T>(T value) => new(value);
}
