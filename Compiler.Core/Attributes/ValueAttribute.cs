using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

/// <summary>
/// This is the old legacy way of caching attributes. It should be replaced with
/// <see cref="GrammarAttribute.Synthetic{TNode,T}(TNode,ref bool,ref T?,System.Func{TNode,T},string)"/>
/// </summary>
// TODO Remove: part of old attribute grammar framework
[StructLayout(LayoutKind.Auto)]
public struct ValueAttribute<T>
{
    private T? value;
    // Declared as a uint to ensure Interlocked can be used on it (without any possible overhead)
    private volatile uint state; // Starts in AttributeState.Pending

    [DebuggerStepThrough]
    public readonly bool TryGetValue(out T value)
    {
        // Volatile read ensures the value read cannot be moved before it
        if (state == (uint)AttributeState.Fulfilled)
        {
            value = this.value!;
            return true;
        }

        value = default!;
        return false;
    }

    [DebuggerStepThrough]
    public T GetValue<TNode>(TNode node, Func<TNode, T> compute)
    {
#pragma warning disable CS0420 // A reference to a volatile field will not be treated as volatile
        var oldState = (AttributeState)Interlocked.CompareExchange(ref state, (uint)AttributeState.InProgress, (uint)AttributeState.Pending);
#pragma warning restore CS0420 // A reference to a volatile field will not be treated as volatile
        switch (oldState)
        {
            default:
                throw ExhaustiveMatch.Failed(oldState);
            case AttributeState.Pending:
                try
                {
                    // We have acquired the right to compute the value
                    value = compute(node);
                    // Volatile write ensures the value write cannot be moved after it
                    state = (byte)AttributeState.Fulfilled;
                    return value!;
                }
                catch
                {
                    // Return the attribute to the pending state so it will be recomputed
                    state = (byte)AttributeState.Pending;
                    throw;
                }
            case AttributeState.InProgress:
                throw new AttributeCycleException();

            case AttributeState.Fulfilled:
                return value!;
        }
    }

    [DebuggerStepThrough]
    public T GetValue(Func<T> compute)
    {
#pragma warning disable CS0420 // A reference to a volatile field will not be treated as volatile
        var oldState = (AttributeState)Interlocked.CompareExchange(ref state, (uint)AttributeState.InProgress, (uint)AttributeState.Pending);
#pragma warning restore CS0420 // A reference to a volatile field will not be treated as volatile
        switch (oldState)
        {
            default:
                throw ExhaustiveMatch.Failed(oldState);
            case AttributeState.Pending:
                try
                {
                    // We have acquired the right to compute the value
                    value = compute();
                    // Volatile write ensures the value write cannot be moved after it
                    state = (byte)AttributeState.Fulfilled;
                    return value!;
                }
                catch
                {
                    // Return the attribute to the pending state so it will be recomputed
                    state = (byte)AttributeState.Pending;
                    throw;
                }
            case AttributeState.InProgress:
                throw new AttributeCycleException();

            case AttributeState.Fulfilled:
                return value!;
        }
    }
}
