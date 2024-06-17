using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

/// <summary>
/// Functions for working with a cached attribute in and attribute grammar.
/// </summary>
public static class GrammarAttribute
{
    /// <remarks><see cref="ThreadStaticAttribute"/> does not support static initializers. The
    /// initializer will be run once and other threads will see the default value. Instead,
    /// <see cref="LazyInitializer"/> is used to ensure it is initialized.</remarks>
    [ThreadStatic]
    private static AttributeGrammarThreadState? _threadStateStorage;

    /// <summary>
    /// Get the thread state for the current thread.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static AttributeGrammarThreadState ThreadState()
        // Do not need to use LazyInitializer here because this is thread static
        => _threadStateStorage ??= new();

    /// <summary>
    /// A flag value used to indicate that a circular attribute has not been set initialized.
    /// </summary>
    public static readonly object Unset = UnsetAttribute.Instance;

    /// <summary>
    /// Safely check whether the attribute has been cached. If it has been, then it is safe to
    /// simply read the attribute value from the backing field.
    /// </summary>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCached(in bool cached) => Volatile.Read(in cached);

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFinal(IChild? child) => child?.IsFinal ?? true;

    #region Synthetic overloads
    /// <summary>
    /// Read the value of a non-circular synthetic attribute that is <see cref="IEquatable{T}"/>.
    /// </summary>
    [DebuggerStepThrough]
    public static T Synthetic<TNode, T>(
        ref bool cached,
        TNode node,
        Func<TNode, T> compute,
        ref T? value,
        [CallerMemberName] string attributeName = "")
        where TNode : class
        where T : class?
        => Synthetic(ref cached, node, compute, StrictEqualityComparer<T>.Instance, ref value, attributeName);

    /// <summary>
    /// Read the value of a non-circular synthetic attribute.
    /// </summary>
    public static T Synthetic<TNode, T>(
        ref bool cached,
        TNode node,
        Func<TNode, T> compute,
        IEqualityComparer<T> comparer,
        ref T? value,
        [CallerMemberName] string attributeName = "")
        where TNode : class
        where T : class?
    {
        if (string.IsNullOrEmpty(attributeName))
            throw new ArgumentException("The attribute name must be provided.", nameof(attributeName));

        var threadState = ThreadState();
        var attributeId = new AttributeId(node, attributeName);
        if (threadState.InCircle)
        {
            if (threadState.ObservedInCycle(attributeId))
                return value!;

            // Do not set the iteration until the value is computed and set so that a value from
            // this cycle is used. Note: non-circular attributes don't have valid initial values.
            var previous = value;
            T next;
            // This context is used to detect whether the attribute depends on a circular or
            // possibly non-final attribute value. If it does, then the value is not cached.
            using (var context = threadState.DependencyContext())
            {
                next = compute(node); // may throw
                if (context.IsFinal)
                {
                    value = next;
                    Volatile.Write(ref cached, true);
                }
            }
            if (!comparer.Equals(next, previous)) // may throw
            {
                var original = Interlocked.CompareExchange(ref value!, next, previous);
                if (!ReferenceEquals(original, previous))
                    next = Unsafe.As<T>(original);
                else
                    // Value updated for this cycle, so update the iteration
                    threadState.UpdateIterationFor(attributeId);
                previous = next;
            }
            else
            {
                // previous == next, so use old value to avoid duplicate objects referenced. Value
                // is correct for this cycle, so update the iteration.
                threadState.UpdateIterationFor(attributeId);
            }

            return previous!;
        }

#if DEBUG
        using var _ = threadState.BeginComputing(attributeId);
#endif
        value = compute(node); // may throw
        Volatile.Write(ref cached, true);
        return value;
    }
    #endregion

    #region Inherited overloads
    /// <summary>
    /// Read the value of a non-circular inherited attribute that is <see cref="IEquatable{T}"/>.
    /// </summary>
    [DebuggerStepThrough]
    public static T Inherited<TNode, T>(
        ref bool cached,
        TNode node,
        Func<IInheritanceContext, T> compute,
        ref T? value,
        [CallerMemberName] string attributeName = "")
        where TNode : class
        where T : class?
        => Inherited(ref cached, node, compute, StrictEqualityComparer<T>.Instance, ref value, attributeName);

    /// <summary>
    /// Read the value of a non-circular inherited attribute.
    /// </summary>
    public static T Inherited<TNode, T>(
        ref bool cached,
        TNode node,
        Func<IInheritanceContext, T> compute,
        IEqualityComparer<T> comparer,
        ref T? value,
        [CallerMemberName] string attributeName = "")
        where TNode : class
        where T : class?
    {
        if (string.IsNullOrEmpty(attributeName))
            throw new ArgumentException("The attribute name must be provided.", nameof(attributeName));

        var threadState = ThreadState();
        var attributeId = new AttributeId(node, attributeName);
        if (threadState.InCircle)
        {
            if (threadState.ObservedInCycle(attributeId))
                return value!;

            // Do not set the iteration until the value is computed and set so that a value from
            // this cycle is used. Note: non-circular attributes don't have valid initial values.
            var previous = value;
            T next;
            // This context is used to detect whether the attribute depends on a circular or
            // possibly non-final attribute value. If it does, then the value is not cached.
            using (var context = threadState.DependencyContext())
            {
                next = compute(threadState); // may throw
                if (context.IsFinal)
                {
                    value = next;
                    Volatile.Write(ref cached, true);
                }
            }
            if (!comparer.Equals(next, previous)) // may throw
            {
                var original = Interlocked.CompareExchange(ref value!, next, previous);
                if (!ReferenceEquals(original, previous))
                    next = Unsafe.As<T>(original);
                else
                    // Value updated for this cycle, so update the iteration
                    threadState.UpdateIterationFor(attributeId);
                previous = next;
            }
            else
            {
                // previous == next, so use old value to avoid duplicate objects referenced. Value
                // is correct for this cycle, so update the iteration.
                threadState.UpdateIterationFor(attributeId);
            }

            return previous!;
        }

#if DEBUG
        using var _ = threadState.BeginComputing(attributeId);
#endif
        value = compute(threadState); // may throw
        Volatile.Write(ref cached, true);
        return value;
    }
    #endregion

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadCircular<T>(in bool cached, in object? storage, out T value)
        where T : class?
    {
        if (Volatile.Read(in cached))
        {
            value = Unsafe.As<T>(storage!);
            return true;
        }

        value = null!;
        return false;
    }

    #region Circular overloads
    /// <summary>
    /// Read the value of a circular attribute that already has an initial value and is
    /// <see cref="IEquatable{T}"/>.
    /// </summary>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Circular<TNode, T>(
        ref bool cached,
        TNode node,
        Func<TNode, T> compute,
        ref object? value,
        [CallerMemberName] string attributeName = "")
        where TNode : class
        where T : class?
        => Circular(ref cached, node, compute, null!, StrictEqualityComparer<T>.Instance, ref value, attributeName);

    /// <summary>
    /// Read the value of a circular attribute that already has an initial value.
    /// </summary>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Circular<TNode, T, TCompare>(
        ref bool cached,
        TNode node,
        Func<TNode, T> compute,
        IEqualityComparer<TCompare> comparer,
        ref object? value,
        [CallerMemberName] string attributeName = "")
        where TNode : class
        where T : class?, TCompare
        => Circular(ref cached, node, compute, null!, comparer, ref value, attributeName);

    /// <summary>
    /// Read the value of a circular attribute that is <see cref="IEquatable{T}"/>.
    /// </summary>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Circular<TNode, T>(
        ref bool cached,
        TNode node,
        Func<TNode, T> compute,
        Func<TNode, T> initializer,
        ref object? value,
        [CallerMemberName] string attributeName = "")
        where TNode : class
        where T : class?
        => Circular(ref cached, node, compute, initializer, StrictEqualityComparer<T>.Instance, ref value, attributeName);

    /// <summary>
    /// Read the value of a circular attribute.
    /// </summary>
    [DebuggerStepThrough]
    public static T Circular<TNode, T, TCompare>(
        ref bool cached,
        TNode node,
        Func<TNode, T> compute,
        Func<TNode, T> initializer,
        IEqualityComparer<TCompare> comparer,
        ref object? value,
        [CallerMemberName] string attributeName = "")
        where TNode : class
        where T : class?, TCompare?
    {
        if (string.IsNullOrEmpty(attributeName))
            throw new ArgumentException("The attribute name must be provided.", nameof(attributeName));

        // Since we must read `value`, go ahead and check the `cached` again in case it was set to true
        if (Volatile.Read(in cached))
            return Unsafe.As<T>(value)!;

        var initial = value;
        if (ReferenceEquals(value, Unset))
        {
            if (initializer is null)
                throw new InvalidOperationException("Attribute not initialized and no initializer provided");
            initial = initializer(node); // may throw
            var original = Interlocked.CompareExchange(ref value, initial, Unset);
            if (original != Unset)
                initial = original;
        }

        T current = Unsafe.As<T>(initial)!;
        var threadState = ThreadState();
        var attributeId = new AttributeId(node, attributeName);
        if (!threadState.InCircle)
        {
            // Using ensures circle is exited when done, making this exception safe.
            using var _ = threadState.EnterCircle();
            bool isFinal;
            do
            {
                threadState.NextIteration();
                isFinal = ComputeCircular(in cached, node, compute, comparer, ref current, ref value, threadState, attributeId);
            } while (threadState.Changed && !isFinal);
            Volatile.Write(ref cached, true);
            return current;
        }

        if (!threadState.ObservedInCycle(attributeId))
        {
            var isFinal = ComputeCircular(in cached, node, compute, comparer, ref current, ref value, threadState, attributeId);
            if (isFinal)
            {
                Volatile.Write(ref cached, true);
                return current;
            }
        }
        // else reuse current approximation

        // The value returned is not the final value, but the value for this cycle
        threadState.MarkNonFinal();
        return current;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool ComputeCircular<TNode, T, TCompare>(
        in bool cached,
        TNode node,
        Func<TNode, T> compute,
        IEqualityComparer<TCompare> comparer,
        ref T current,
        ref object? value,
        AttributeGrammarThreadState threadState,
        AttributeId attributeId)
        where TNode : class
        where T : class?, TCompare?
    {
        // Set to current iteration before computing so a cycle will use the previous value
        threadState.UpdateIterationFor(attributeId);
        T? next;
        bool isFinal;
        // This context is used to detect whether the attribute depends on a circular or
        // possibly non-final attribute value. If it doesn't, then the value can be cached.
        using (var ctx = threadState.DependencyContext())
        {
            next = compute(node); // may throw
            isFinal = ctx.IsFinal;
        }

        if (comparer.Equals(current, next)) // may throw
            // current == next, so use old value to avoid duplicate objects referenced
            return isFinal;

        threadState.MarkChanged();
        var original = Interlocked.CompareExchange(ref value, next, current);
        if (!ReferenceEquals(original, current))
        {
            // The value was changed by another thread, so use the new value. First though, check
            // whether it is cached and therefore final.
            isFinal = Volatile.Read(in cached);
            if (isFinal)
                // Read again if final to ensure the value is the one that is actually cached
                original = value;
            next = Unsafe.As<T>(original)!;
        }
        current = next;
        return isFinal;
    }
    #endregion

    #region Child
    /// <summary>
    /// Read the value of a circular attribute.
    /// </summary>
    [DebuggerStepThrough]
    public static TChild Child<TNode, TChild, TParent>(
        TNode node,
        ref TChild child,
        [CallerMemberName] string attributeName = "")
        where TNode : class, TParent, IParent
        where TChild : class?, IChild<TParent>?
    {
        if (string.IsNullOrEmpty(attributeName))
            throw new ArgumentException("The attribute name must be provided.", nameof(attributeName));

        TChild current = child;
        if (current is null || current.IsFinal)
            return current!;

        var threadState = ThreadState();
        var attributeId = new AttributeId(node, attributeName);
        if (!threadState.InCircle)
        {
            // Using ensures circle is exited when done, making this exception safe.
            using var _ = threadState.EnterCircle();
            bool isFinal;
            do
            {
                threadState.NextIteration();
                isFinal = ComputeChild<TChild, TParent>(ref child, ref current, threadState, attributeId);
            } while (threadState.Changed && !isFinal);
            current.MarkFinal();
            return current;
        }

        if (!threadState.ObservedInCycle(attributeId))
        {
            var isFinal = ComputeChild<TChild, TParent>(ref child, ref current, threadState, attributeId);
            if (isFinal && node.IsFinal)
            {
                current.MarkFinal();
                return current;
            }
        }
        // else reuse current approximation

        // The value returned is not the final value, but the value for this cycle
        threadState.MarkNonFinal();
        return current;
    }

    private static bool ComputeChild<TChild, TParent>(
        ref TChild child,
        [NotNull][DisallowNull] ref TChild previous,
        AttributeGrammarThreadState threadState,
        AttributeId attributeId)
        where TChild : class?, IChild<TParent>?
    {
        // Set to current iteration before computing so a cycle will use the previous value
        threadState.UpdateIterationFor(attributeId);

        bool isFinal;
        TChild? next; // may throw
        using (var ctx = threadState.DependencyContext())
        {
            next = (TChild?)previous.Rewrite();
            isFinal = ctx.IsFinal;
        }

        if (next is not null)
        {
            threadState.MarkChanged();
            var original = Interlocked.CompareExchange(ref child, next, previous);
            if (!ReferenceEquals(original, previous))
            {
                next = original!; // original should never be null because you can't rewrite to null
                isFinal = next.IsFinal;
            }
            previous = next;
        }
        // else no rewrite

        return isFinal;
    }
    #endregion
}
