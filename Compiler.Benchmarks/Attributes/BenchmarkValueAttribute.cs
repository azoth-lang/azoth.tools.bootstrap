using System.Runtime.InteropServices;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Benchmarks.Attributes;

[StructLayout(LayoutKind.Auto)]
public struct BenchmarkValueAttribute<T>
{
    private T? value;
    // Declared as a uint to ensure Interlocked can be used on it (without any possible overhead)
    private volatile uint state; // Starts in Pending

    /// <summary>
    /// Reset the attribute to its initial state.
    /// </summary>
    public void Reset()
    {
        value = default;
        // Volatile write ensures the value write cannot be moved after it
        state = (uint)BenchmarkAttributeState.Pending;
    }

    public readonly bool TryGetValue(out T value)
    {
        // Volatile read ensures the value read cannot be moved before it
        if (state == (uint)BenchmarkAttributeState.Fulfilled)
        {
            value = this.value!;
            return true;
        }

        value = default!;
        return false;
    }

    public T GetValueSimple<TNode>(TNode node, Func<TNode, T> compute)
    {
#pragma warning disable CS0420 // A reference to a volatile field will not be treated as volatile
        var oldState = (BenchmarkAttributeState)Interlocked.CompareExchange(ref state, (uint)BenchmarkAttributeState.InProgress, (uint)BenchmarkAttributeState.Pending);
#pragma warning restore CS0420 // A reference to a volatile field will not be treated as volatile
        switch (oldState)
        {
            default:
                throw ExhaustiveMatch.Failed(oldState);
            case BenchmarkAttributeState.Pending:
                try
                {
                    // We have acquired the right to compute the value
                    value = compute(node);
                    // Volatile write ensures the value write cannot be moved after it
                    state = (byte)BenchmarkAttributeState.Fulfilled;
                    return value!;
                }
                catch
                {
                    // Return the attribute to the pending state so it will be recomputed
                    state = (byte)BenchmarkAttributeState.Pending;
                    throw;
                }
            case BenchmarkAttributeState.InProgress:
                throw new BenchmarkAttributeCycleException();

            case BenchmarkAttributeState.Fulfilled:
                return value!;
        }
    }

    public T GetValueSimple(Func<T> compute)
    {
#pragma warning disable CS0420 // A reference to a volatile field will not be treated as volatile
        var oldState = (BenchmarkAttributeState)Interlocked.CompareExchange(ref state, (uint)BenchmarkAttributeState.InProgress, (uint)BenchmarkAttributeState.Pending);
#pragma warning restore CS0420 // A reference to a volatile field will not be treated as volatile
        switch (oldState)
        {
            default:
                throw ExhaustiveMatch.Failed(oldState);
            case BenchmarkAttributeState.Pending:
                try
                {
                    // We have acquired the right to compute the value
                    value = compute();
                    // Volatile write ensures the value write cannot be moved after it
                    state = (byte)BenchmarkAttributeState.Fulfilled;
                    return value!;
                }
                catch
                {
                    // Return the attribute to the pending state so it will be recomputed
                    state = (byte)BenchmarkAttributeState.Pending;
                    throw;
                }
            case BenchmarkAttributeState.InProgress:
                throw new BenchmarkAttributeCycleException();

            case BenchmarkAttributeState.Fulfilled:
                return value!;
        }
    }

    public T GetValueSmart<TNode>(TNode node, Func<TNode, T> compute)
    {
        // Volatile read ensures the value read cannot be moved before it
        if (state == (uint)BenchmarkAttributeState.Fulfilled)
            return value!;

#pragma warning disable CS0420 // A reference to a volatile field will not be treated as volatile
        var oldState = (BenchmarkAttributeState)Interlocked.CompareExchange(ref state, (uint)BenchmarkAttributeState.InProgress, (uint)BenchmarkAttributeState.Pending);
#pragma warning restore CS0420 // A reference to a volatile field will not be treated as volatile
        switch (oldState)
        {
            default:
                throw ExhaustiveMatch.Failed(oldState);
            case BenchmarkAttributeState.Pending:
                try
                {
                    // We have acquired the right to compute the value
                    value = compute(node);
                    // Volatile write ensures the value write cannot be moved after it
                    state = (byte)BenchmarkAttributeState.Fulfilled;
                    return value!;
                }
                catch
                {
                    // Return the attribute to the pending state so it will be recomputed
                    state = (byte)BenchmarkAttributeState.Pending;
                    throw;
                }
            case BenchmarkAttributeState.InProgress:
                throw new BenchmarkAttributeCycleException();

            case BenchmarkAttributeState.Fulfilled:
                return value!;
        }
    }

    public T GetValueSmart(Func<T> compute)
    {
        // Volatile read ensures the value read cannot be moved before it
        if (state == (uint)BenchmarkAttributeState.Fulfilled)
            return value!;

#pragma warning disable CS0420 // A reference to a volatile field will not be treated as volatile
        var oldState = (BenchmarkAttributeState)Interlocked.CompareExchange(ref state, (uint)BenchmarkAttributeState.InProgress, (uint)BenchmarkAttributeState.Pending);
#pragma warning restore CS0420 // A reference to a volatile field will not be treated as volatile
        switch (oldState)
        {
            default:
                throw ExhaustiveMatch.Failed(oldState);
            case BenchmarkAttributeState.Pending:
                try
                {
                    // We have acquired the right to compute the value
                    value = compute();
                    // Volatile write ensures the value write cannot be moved after it
                    state = (byte)BenchmarkAttributeState.Fulfilled;
                    return value!;
                }
                catch
                {
                    // Return the attribute to the pending state so it will be recomputed
                    state = (byte)BenchmarkAttributeState.Pending;
                    throw;
                }
            case BenchmarkAttributeState.InProgress:
                throw new BenchmarkAttributeCycleException();

            case BenchmarkAttributeState.Fulfilled:
                return value!;
        }
    }
}
