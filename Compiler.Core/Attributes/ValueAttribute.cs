using System;
using System.Runtime.CompilerServices;
using System.Threading;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

public struct ValueAttribute<T>
{
    private T? value;
    private AttributeState state; // Starts in Pending

    public bool TryGetValue(out T value)
    {
        if (VolatileRead(ref state) == AttributeState.Fulfilled)
        {
            value = this.value!;
            return true;
        }

        value = default!;
        return false;
    }

    public T GetValue<TNode>(TNode node, Func<TNode, T> compute)
    {
        var oldState = InterlockedCompareExchange(ref state, AttributeState.InProgress, AttributeState.Pending);
        switch (oldState)
        {
            default:
                throw ExhaustiveMatch.Failed(oldState);
            case AttributeState.Pending:
                // We have acquired the right to compute the value
                value = compute(node);
                state = AttributeState.Fulfilled;
                return value!;
            case AttributeState.InProgress:
                throw new InvalidOperationException("Cyclic dependency detected.");

            case AttributeState.Fulfilled:
                return value!;
        }
    }

    public T GetValue(Func<T> compute)
    {
        var oldState = InterlockedCompareExchange(ref state, AttributeState.InProgress, AttributeState.Pending);
        switch (oldState)
        {
            default:
                throw ExhaustiveMatch.Failed(oldState);
            case AttributeState.Pending:
                // We have acquired the right to compute the value
                value = compute();
                state = AttributeState.Fulfilled;
                return value!;
            case AttributeState.InProgress:
                throw new InvalidOperationException("Cyclic dependency detected.");

            case AttributeState.Fulfilled:
                return value!;
        }
    }

    private static AttributeState InterlockedCompareExchange(ref AttributeState location, AttributeState value, AttributeState comparand)
    {
        var result = Interlocked.CompareExchange(ref Unsafe.As<AttributeState, int>(ref location), (int)value, (int)comparand);
        return (AttributeState)result;
    }

    private static AttributeState VolatileRead(ref AttributeState location)
    {
        int result = Volatile.Read(ref Unsafe.As<AttributeState, int>(ref location));
        return (AttributeState)result;
    }
}
