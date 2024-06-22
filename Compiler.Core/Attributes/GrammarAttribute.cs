using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;
using Azoth.Tools.Bootstrap.Framework;
using InlineMethod;
using Void = Azoth.Tools.Bootstrap.Framework.Void;

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

    private static Void _noLock;

    /// <summary>
    /// Get the thread state for the current thread.
    /// </summary>
    [Inline] // Not always working
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static AttributeGrammarThreadState ThreadState()
        // Do not need to use LazyInitializer here because this is thread static
        => _threadStateStorage ??= new();

    /// <summary>
    /// Safely check whether the attribute has been cached. If it has been, then it is safe to
    /// simply read the attribute value from the backing field.
    /// </summary>
    [Inline] // Not always working
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCached(in bool cached) => Volatile.Read(in cached);

    [Inline] // Not always working
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFinal(IChild? child) => child?.IsFinal ?? true;

    /// <summary>
    /// Get the inheritance context for the current thread.
    /// </summary>
    /// <remarks>This should only be used for nodes that directly expose a function that calls the
    /// inherited member.</remarks>
    [Inline] // Not always working
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IInheritanceContext CurrentInheritanceContext() => ThreadState();

    #region NonCircular overloads
    /// <summary>
    /// Read the value of a non-circular attribute.
    /// </summary>
    public static T NonCircular<TNode, T, TFunc, TOp, TLock>(
        this TNode node,
        ref bool cached,
        ref T? value,
        TFunc func,
        IEqualityComparer<T> comparer,
        ref TLock syncLock,
        [CallerMemberName] string attributeName = "")
        where TNode : class, IParent
        where TFunc : struct, IAttributeFunction<TNode, T>
        where TOp : IAttributeOperations<T, TLock>
        where TLock : struct
    {
        if (string.IsNullOrEmpty(attributeName))
            throw new ArgumentException("The attribute name must be provided.", nameof(attributeName));

        var threadState = ThreadState();
        var attributeId = new AttributeId(node, attributeName);
        T next;
        if (threadState.InCircle)
        {
            if (threadState.ObservedInCycle(attributeId))
            {
                // Since the value wasn't cached, it must not be final
                threadState.MarkNonFinal();
                return TOp.Read(in value!, ref syncLock);
            }

            // Do not set the iteration until the value is computed and set so that a value from
            // this cycle is used. Note: non-circular attributes don't have valid initial values.
            var previous = TOp.Read(in value, ref syncLock);
            // This context is used to detect whether the attribute depends on a circular or
            // possibly non-final attribute value. If it does, then the value is not cached.
            using (var context = threadState.DependencyContext())
            {
                next = func.Compute(node, threadState); // may throw
                if (context.IsFinal)
                    return TOp.WriteFinal(ref value, next, ref syncLock, ref cached);
            }
            if (!comparer.Equals(next, previous)) // may throw
            {
                if (!TOp.CompareExchange(ref value, next, previous, comparer, ref syncLock, out var original)) // may throw
                    next = original!;
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
        next = func.Compute(node, threadState); // may throw
        return TOp.WriteFinal(ref value, next, ref syncLock, ref cached);
    }

    [Inline]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T NonCircular<TNode, T, TFunc>(
        this TNode node,
        ref bool cached,
        ref T? value,
        TFunc func,
        IEqualityComparer<T> comparer,
        string attributeName)
        where TNode : class, IParent
        where TFunc : struct, IAttributeFunction<TNode, T>
        where T : class?
        => node.NonCircular<TNode, T, TFunc, ReferenceOperations<T>, Void>(ref cached, ref value,
            func, comparer, ref _noLock, attributeName);

    [Inline]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T NonCircular<TNode, T, TFunc>(
        this TNode node,
        ref bool cached,
        ref T value,
        TFunc func,
        IEqualityComparer<T> comparer,
        ref AttributeLock syncLock,
        string attributeName)
        where TNode : class, IParent
        where TFunc : struct, IAttributeFunction<TNode, T>
        where T : struct
        => node.NonCircular<TNode, T, TFunc, ValueOperations<T>, AttributeLock>(ref cached, ref value,
            func, comparer, ref syncLock, attributeName);

    [Inline]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T? NonCircular<TNode, T, TFunc>(
        this TNode node,
        ref bool cached,
        ref T? value,
        TFunc func,
        IEqualityComparer<T?> comparer,
        ref AttributeLock syncLock,
        string attributeName)
        where TNode : class, IParent
        where TFunc : struct, IAttributeFunction<TNode, T?>
        where T : struct
        => node.NonCircular<TNode, T?, TFunc, ValueOperations<T?>, AttributeLock>(ref cached, ref value,
            func, comparer, ref syncLock, attributeName);
    #endregion

    #region Synthetic overloads
    /// <summary>
    /// Read the value of a non-circular synthetic attribute that is <see cref="IEquatable{T}"/> for
    /// some supertype.
    /// </summary>
    [Inline] // Not always working
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Synthetic<TNode, T>(
        this TNode node,
        ref bool cached,
        ref T? value,
        Func<TNode, T> compute,
        [CallerMemberName] string attributeName = "")
        where TNode : class, IParent
        where T : class?
        => node.NonCircular(ref cached, ref value, AttributeFunction.Create(compute),
            StrictEqualityComparer<T>.Instance, attributeName);

    /// <summary>
    /// Read the value of a non-circular synthetic attribute.
    /// </summary>
    [Inline] // Not always working
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Synthetic<TNode, T>(
        this TNode node,
        ref bool cached,
        ref T? value,
        Func<TNode, T> compute,
        IEqualityComparer<T> comparer,
        [CallerMemberName] string attributeName = "")
        where TNode : class, IParent
        where T : class?
        => node.NonCircular(ref cached, ref value, AttributeFunction.Create(compute), comparer, attributeName);

    /// <summary>
    /// Read the value of a non-circular synthetic attribute that is <see cref="IEquatable{T}"/>.
    /// </summary>
    [Inline] // Not always working
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Synthetic<TNode, T>(
        this TNode node,
        ref bool cached,
        ref T value,
        ref AttributeLock syncLock,
        Func<TNode, T> compute,
        [CallerMemberName] string attributeName = "")
        where TNode : class, IParent
        where T : struct
        => node.NonCircular(ref cached, ref value, AttributeFunction.Create(compute),
            StrictEqualityComparer<T>.Instance, ref syncLock, attributeName);

    /// <summary>
    /// Read the value of a non-circular synthetic attribute that is <see cref="IEquatable{T}"/>.
    /// </summary>
    [Inline] // Not always working
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Synthetic<TNode, T>(
        this TNode node,
        ref bool cached,
        ref T? value,
        ref AttributeLock syncLock,
        Func<TNode, T?> compute,
        [CallerMemberName] string attributeName = "")
        where TNode : class, IParent
        where T : struct
        => node.NonCircular(ref cached, ref value, AttributeFunction.Create(compute),
            StrictEqualityComparer<T?>.Instance, ref syncLock, attributeName);
    #endregion

    #region Inherited overloads
    /// <summary>
    /// Read the value of a non-circular inherited attribute that is <see cref="IEquatable{T}"/>.
    /// </summary>
    [Inline]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Inherited<TNode, T>(
        this TNode node,
        ref bool cached,
        ref T? value,
        Func<IInheritanceContext, T> compute,
        [CallerMemberName] string attributeName = "")
        where TNode : class, IParent
        where T : class?
        => node.NonCircular(ref cached, ref value, AttributeFunction.Create<TNode, T>(compute),
            StrictEqualityComparer<T>.Instance, attributeName);

    /// <summary>
    /// Read the value of a non-circular inherited attribute.
    /// </summary>
    [Inline]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Inherited<TNode, T>(
        this TNode node,
        ref bool cached,
        ref T? value,
        Func<IInheritanceContext, T> compute,
        IEqualityComparer<T> comparer,
        [CallerMemberName] string attributeName = "")
        where TNode : class, IParent
        where T : class?
        => node.NonCircular(ref cached, ref value, AttributeFunction.Create<TNode, T>(compute),
            comparer, attributeName);
    #endregion

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
        ref Circular<T> value,
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
        ref Circular<T> value,
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
        ref Circular<T> value,
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
        ref Circular<T> value,
        [CallerMemberName] string attributeName = "")
        where TNode : class
        where T : class?, TCompare?
    {
        if (string.IsNullOrEmpty(attributeName))
            throw new ArgumentException("The attribute name must be provided.", nameof(attributeName));

        // Since we must read `value`, go ahead and check the `cached` again in case it was set to true
        if (Volatile.Read(in cached))
            return value.UnsafeValue;

        if (!value.IsInitialized)
        {
            if (initializer is null)
                throw new InvalidOperationException("Attribute not initialized and no initializer provided");
            value.Initialize(initializer(node)); // initializer may throw
        }

        T current = value.UnsafeValue;
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
        ref Circular<T> value,
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
        var original = value.CompareExchange(next, current);
        if (!ReferenceEquals(original, current))
        {
            // The value was changed by another thread, so use the new value. First though, check
            // whether it is cached and therefore final.
            isFinal = Volatile.Read(in cached);
            if (isFinal)
                // Read again if final to ensure the value is the one that is actually cached
                original = value.UnsafeValue;
            next = original;
        }
        current = next;
        return isFinal;
    }
    #endregion

    #region Child
    /// <summary>
    /// Read the value of a circular attribute.
    /// </summary>
    //[DebuggerStepThrough]
    public static TChild Child<TNode, TChild>(
        TNode node,
        ref TChild child,
        [CallerMemberName] string attributeName = "")
        where TNode : class, IParent
        where TChild : class?, IChild<TNode>?
    {
        if (string.IsNullOrEmpty(attributeName))
            throw new ArgumentException("The attribute name must be provided.", nameof(attributeName));

        TChild current = child;
        if (current is null || current.IsFinal)
            return current;

        if (!current.MayHaveRewrite)
        {
            if (node.IsFinal)
                current.MarkFinal();
            return current;
        }

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
                isFinal = ComputeChild(node, ref child, ref current, threadState, attributeId);
            } while (threadState.Changed && !isFinal && current.MayHaveRewrite);
            if (node.IsFinal)
                current.MarkFinal();
            return current;
        }

        if (!threadState.ObservedInCycle(attributeId))
        {
            var isFinal = ComputeChild(node, ref child, ref current, threadState, attributeId);
            if (isFinal)
                return current;
        }
        // else reuse current approximation

        // The value returned is not the final value, but the value for this cycle
        threadState.MarkNonFinal();
        return current;
    }

    private static bool ComputeChild<TNode, TChild>(
        TNode node,
        ref TChild child,
        [NotNull][DisallowNull] ref TChild current,
        AttributeGrammarThreadState threadState,
        AttributeId attributeId)
        where TChild : class?, IChild<TNode>?
    {
        // Set to current iteration before computing so a cycle will use the previous value
        threadState.UpdateIterationFor(attributeId);

        // Rewrites do not use the dependency context because even if they don't depend on something
        // that is not final, they may still get rewritten again.

        var next = (TChild?)current.Rewrite(); // may throw

        if (next is not null)
        {
            threadState.MarkChanged();
            var original = Interlocked.CompareExchange(ref child, next, current);
            if (!ReferenceEquals(original, current))
                next = original!; // original should never be null because you can't rewrite to null
            else
                Attributes.Child.AttachRewritten(node, next);
            current = next;
        }
        // else no rewrite

        // If the child is already final (either another thread marked it final or it was marked
        // final by attaching it to the parent since it can't be rewritten), then it is final. Even
        // if Rewrite() returns null, the child may not be final if it depends on a non-cached attribute.
        return next?.IsFinal ?? false;
    }
    #endregion
}
