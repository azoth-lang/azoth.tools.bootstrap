using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;
using Azoth.Tools.Bootstrap.Framework;
using InlineMethod;
using JetBrains.Annotations;
using Void = Azoth.Tools.Bootstrap.Framework.Void;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

/// <summary>
/// Functions for working with a cached attribute in and attribute grammar.
/// </summary>
[DebuggerStepThrough]
public static class GrammarAttribute
{
    /// <remarks><see cref="ThreadStaticAttribute"/> does not support static initializers. The
    /// initializer will be run once and other threads will see the default value. Instead,
    /// <see cref="ThreadState"/> is used to ensure it is initialized.</remarks>
    [ThreadStatic]
    private static AttributeGrammarThreadState? _threadStateStorage;

    private static Void _noLock;

    /// <summary>
    /// Get the thread state for the current thread.
    /// </summary>
    [Inline]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static AttributeGrammarThreadState ThreadState()
        // Do not need to use LazyInitializer here because this is thread static
        => _threadStateStorage ??= new();

    /// <summary>
    /// Safely check whether the attribute has been cached. If it has been, then it is safe to
    /// simply read the attribute value from the backing field.
    /// </summary>
    [Inline(export: true)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCached(in bool cached) => Volatile.Read(in cached);

    /// <summary>
    /// Get the inheritance context for the current thread.
    /// </summary>
    /// <remarks>This should only be used for nodes that directly expose a function that calls the
    /// inherited member.</remarks>
    [Inline]
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
        where TNode : class, ITreeNode
        where TFunc : struct, IAttributeFunction<TNode, T>
        where TOp : IAttributeOperations<T, TLock>
        where TLock : struct
    {
        if (string.IsNullOrEmpty(attributeName))
            throw new ArgumentException("The attribute name must be provided.", nameof(attributeName));

        var threadState = ThreadState();
        var attributeId = new AttributeId(node, attributeName);
        var current = TOp.Read(in value!, ref syncLock);
        if (threadState.ComputedInCurrentIteration(attributeId))
            // TODO must set low link correctly
            return current;

        using var attributeScope = threadState.VisitNonCircular(attributeId);
        var next = func.Compute(node, threadState); // may throw
        if (attributeScope.IsFinal)
        {
            attributeScope.RemoveComputedInIteration();
            return TOp.WriteFinal(ref value, next, ref syncLock, ref cached);
        }

        if (!comparer.Equals(next, current)) // may throw
        {
            if (!TOp.CompareExchange(ref value, next, current, comparer, ref syncLock, out var original)) // may throw
                next = original!;
            else
                // Value updated for this cycle, so update the iteration
                attributeScope.MarkComputedInCurrentIteration();
            current = next;
        }

        return current;
    }

    [Inline(export: true)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T NonCircular<TNode, T, TFunc>(
        this TNode node,
        ref bool cached,
        ref T? value,
        TFunc func,
        IEqualityComparer<T> comparer,
        string attributeName)
        where TNode : class, ITreeNode
        where TFunc : struct, IAttributeFunction<TNode, T>
        where T : class?
        => node.NonCircular<TNode, T, TFunc, ReferenceOperations<T>, Void>(ref cached, ref value,
            func, comparer, ref _noLock, attributeName);

    [Inline(export: true)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T NonCircular<TNode, T, TFunc>(
        this TNode node,
        ref bool cached,
        ref T value,
        TFunc func,
        IEqualityComparer<T> comparer,
        ref AttributeLock syncLock,
        string attributeName)
        where TNode : class, ITreeNode
        where TFunc : struct, IAttributeFunction<TNode, T>
        where T : struct
        => node.NonCircular<TNode, T, TFunc, ValueOperations<T>, AttributeLock>(ref cached, ref value,
            func, comparer, ref syncLock, attributeName);

    [Inline(export: true)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T? NonCircular<TNode, T, TFunc>(
        this TNode node,
        ref bool cached,
        ref T? value,
        TFunc func,
        IEqualityComparer<T?> comparer,
        ref AttributeLock syncLock,
        string attributeName)
        where TNode : class, ITreeNode
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Synthetic<TNode, T>(
        this TNode node,
        ref bool cached,
        ref T? value,
        [RequireStaticDelegate] Func<TNode, T> compute,
        [CallerMemberName] string attributeName = "")
        where TNode : class, ITreeNode
        where T : class?
        => node.NonCircular(ref cached, ref value, AttributeFunction.Create(compute),
            EqualityComparer<T>.Default, attributeName);

    /// <summary>
    /// Read the value of a non-circular synthetic attribute.
    /// </summary>
    [Inline] // Not always working
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Synthetic<TNode, T>(
        this TNode node,
        ref bool cached,
        ref T? value,
        [RequireStaticDelegate] Func<TNode, T> compute,
        IEqualityComparer<T> comparer,
        [CallerMemberName] string attributeName = "")
        where TNode : class, ITreeNode
        where T : class?
        => node.NonCircular(ref cached, ref value, AttributeFunction.Create(compute), comparer, attributeName);

    /// <summary>
    /// Read the value of a non-circular synthetic attribute that is <see cref="IEquatable{T}"/>.
    /// </summary>
    [Inline] // Not always working
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Synthetic<TNode, T>(
        this TNode node,
        ref bool cached,
        ref T value,
        ref AttributeLock syncLock,
        [RequireStaticDelegate] Func<TNode, T> compute,
        [CallerMemberName] string attributeName = "")
        where TNode : class, ITreeNode
        where T : struct
        => node.NonCircular(ref cached, ref value, AttributeFunction.Create(compute),
            EqualityComparer<T>.Default, ref syncLock, attributeName);

    /// <summary>
    /// Read the value of a non-circular synthetic attribute that is <see cref="IEquatable{T}"/>.
    /// </summary>
    [Inline] // Not always working
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Synthetic<TNode, T>(
        this TNode node,
        ref bool cached,
        ref T? value,
        ref AttributeLock syncLock,
        [RequireStaticDelegate] Func<TNode, T?> compute,
        [CallerMemberName] string attributeName = "")
        where TNode : class, ITreeNode
        where T : struct
        => node.NonCircular(ref cached, ref value, AttributeFunction.Create(compute),
            EqualityComparer<T?>.Default, ref syncLock, attributeName);
    #endregion

    #region Inherited overloads
    /// <summary>
    /// Read the value of a non-circular inherited attribute that is <see cref="IEquatable{T}"/>.
    /// </summary>
    [Inline]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Inherited<TNode, T>(
        this TNode node,
        ref bool cached,
        ref T? value,
        [RequireStaticDelegate] Func<IInheritanceContext, T> compute,
        [CallerMemberName] string attributeName = "")
        where TNode : class, ITreeNode
        where T : class?
        => node.NonCircular(ref cached, ref value, AttributeFunction.Create<TNode, T>(compute),
            EqualityComparer<T>.Default, attributeName);

    /// <summary>
    /// Read the value of a non-circular inherited attribute.
    /// </summary>
    [Inline]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Inherited<TNode, T>(
        this TNode node,
        ref bool cached,
        ref T? value,
        [RequireStaticDelegate] Func<IInheritanceContext, T> compute,
        IEqualityComparer<T> comparer,
        [CallerMemberName] string attributeName = "")
        where TNode : class, ITreeNode
        where T : class?
        => node.NonCircular(ref cached, ref value, AttributeFunction.Create<TNode, T>(compute), comparer,
            attributeName);

    /// <summary>
    /// Read the value of a non-circular inherited attribute that is <see cref="IEquatable{T}"/>.
    /// </summary>
    [Inline]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Inherited<TNode, T>(
        this TNode node,
        ref bool cached,
        ref T value,
        ref AttributeLock syncLock,
        [RequireStaticDelegate] Func<IInheritanceContext, T> compute,
        [CallerMemberName] string attributeName = "")
        where TNode : class, ITreeNode
        where T : struct
        => node.NonCircular(ref cached, ref value, AttributeFunction.Create<TNode, T>(compute),
            EqualityComparer<T>.Default, ref syncLock, attributeName);

    /// <summary>
    /// Read the value of a non-circular inherited attribute that is <see cref="IEquatable{T}"/>.
    /// </summary>
    [Inline]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Inherited<TNode, T>(
        this TNode node,
        ref bool cached,
        ref T? value,
        ref AttributeLock syncLock,
        [RequireStaticDelegate] Func<IInheritanceContext, T?> compute,
        [CallerMemberName] string attributeName = "")
        where TNode : class, ITreeNode
        where T : struct
        => node.NonCircular(ref cached, ref value, AttributeFunction.Create<TNode, T?>(compute),
            EqualityComparer<T?>.Default, ref syncLock, attributeName);
    #endregion

    #region Aggregate overloads
    /// <summary>
    /// Read the value of an aggregate attribute.
    /// </summary>
    [Inline]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Aggregate<TNode, T>(
        this TNode node,
        ref IFixedSet<TNode>? contributors,
        ref bool cached,
        ref T? value,
        Func<IFixedSet<TNode>> collectContributors,
        Func<IFixedSet<TNode>, T> collect,
        [CallerMemberName] string attributeName = "")
        where TNode : class, ITreeNode
        where T : class?
    {
        var actualContributors = GetAggregateContributors(ref contributors, collectContributors, attributeName);
        if (actualContributors is null)
            // Another thread has already computed the value
            return value!;

        var result = node.NonCircular(ref cached, ref value, AttributeFunction.Create<TNode, T>(Compute),
            EqualityComparer<T>.Default, attributeName);

        if (cached) contributors = null;

        return result;

        T Compute() => collect(actualContributors);
    }

    // Not null inference is wrong here. This can be null because another thread could null it out.
    private static IFixedSet<T>? GetAggregateContributors<T>(
        ref IFixedSet<T>? contributors,
        Func<IFixedSet<T>> collectContributors,
        string attributeName)
        where T : class?
    {
        var actualContributors = Interlocked.CompareExchange(ref contributors, InProgressFixedSet<T>.Instance, null);
        if (actualContributors is null)
        {
            actualContributors = collectContributors();
            // Null inference is wrong here. This can be null because another thread could null it out.
            IFixedSet<T>? previousValue = Interlocked.CompareExchange(ref contributors, actualContributors, InProgressFixedSet<T>.Instance);
            if (!ReferenceEquals(previousValue, InProgressFixedSet<T>.Instance))
                actualContributors = previousValue;
        }
        else if (ReferenceEquals(actualContributors, InProgressFixedSet<T>.Instance))
            throw new InvalidOperationException($"Circular dependency in collecting contributors for aggregate attribute '{attributeName}'.");
        return actualContributors;
    }
    #endregion

    #region Collection overloads
    /// <summary>
    /// Read the value of a collection attribute.
    /// </summary>
    [Inline]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Collection<TNode, T>(
        this TNode node,
        ref IFixedSet<TNode>? contributors,
        ref bool cached,
        ref T? value,
        Func<TNode, IInheritanceContext, IFixedSet<TNode>> collectContributors,
        Func<TNode, IFixedSet<TNode>, T> collect,
        [CallerMemberName] string attributeName = "")
        where TNode : class, ITreeNode
        where T : class?
    {
        var actualContributors = GetCollectionContributors(node, ref contributors, collectContributors, attributeName);
        if (actualContributors is null)
            // Another thread has already computed the value
            return value!;

        var result = node.NonCircular(ref cached, ref value, AttributeFunction.Create<TNode, T>(Compute),
            EqualityComparer<T>.Default, attributeName);

        if (cached) contributors = null;

        return result;

        T Compute(TNode t) => collect(t, actualContributors);
    }

    // Not null inference is wrong here. This can be null because another thread could null it out.
    private static IFixedSet<T>? GetCollectionContributors<T>(
        T node,
        ref IFixedSet<T>? contributors,
        Func<T, IInheritanceContext, IFixedSet<T>> collectContributors,
        string attributeName)
        where T : class?
    {
        var actualContributors = Interlocked.CompareExchange(ref contributors, InProgressFixedSet<T>.Instance, null);
        if (actualContributors is null)
        {
            actualContributors = collectContributors(node, ThreadState());
            // Null inference is wrong here. This can be null because another thread could null it out.
            IFixedSet<T>? previousValue = Interlocked.CompareExchange(ref contributors, actualContributors, InProgressFixedSet<T>.Instance);
            if (!ReferenceEquals(previousValue, InProgressFixedSet<T>.Instance))
                actualContributors = previousValue;
        }
        else if (ReferenceEquals(actualContributors, InProgressFixedSet<T>.Instance))
            throw new InvalidOperationException($"Circular dependency in collecting contributors for collection attribute '{attributeName}'.");
        return actualContributors;
    }
    #endregion


    #region Circular overloads
    /// <summary>
    /// Read the value of a circular attribute that already has an initial value and is
    /// <see cref="IEquatable{T}"/>.
    /// </summary>
    [Inline] // Not always working
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Circular<TNode, T>(
        this TNode node,
        ref bool cached,
        ref Circular<T> value,
        [RequireStaticDelegate] Func<TNode, T> compute,
        [CallerMemberName] string attributeName = "")
        where TNode : class, ITreeNode
        where T : class?
        => node.Cyclic(ref cached, ref value, AttributeFunction.Create(compute), default(Func<TNode, T>),
            EqualityComparer<T>.Default, attributeName);

    /// <summary>
    /// Read the value of a circular attribute that already has an initial value.
    /// </summary>
    [Inline] // Not always working
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Circular<TNode, T, TCompare>(
        this TNode node,
        ref bool cached,
        ref Circular<T> value,
        [RequireStaticDelegate] Func<TNode, T> compute,
        IEqualityComparer<TCompare> comparer,
        [CallerMemberName] string attributeName = "")
        where TNode : class, ITreeNode
        where T : class?, TCompare
        => node.Cyclic(ref cached, ref value, AttributeFunction.Create(compute), default(Func<TNode, T>),
            comparer, attributeName);

    /// <summary>
    /// Read the value of a circular attribute that is <see cref="IEquatable{T}"/>.
    /// </summary>
    [Inline] // Not always working
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Circular<TNode, T>(
        this TNode node,
        ref bool cached,
        ref Circular<T> value,
        [RequireStaticDelegate] Func<TNode, T> compute,
        [RequireStaticDelegate] Func<TNode, T> initializer,
        [CallerMemberName] string attributeName = "")
        where TNode : class, ITreeNode
        where T : class?
        => node.Cyclic(ref cached, ref value, AttributeFunction.Create(compute), initializer,
            EqualityComparer<T>.Default, attributeName);

    /// <summary>
    /// Read the value of a circular attribute.
    /// </summary>
    [Inline] // Not always working
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Circular<TNode, T, TCompare>(
        this TNode node,
        ref bool cached,
        ref Circular<T> value,
        [RequireStaticDelegate] Func<TNode, T> compute,
        [RequireStaticDelegate] Func<TNode, T> initializer,
        IEqualityComparer<TCompare> comparer,
        [CallerMemberName] string attributeName = "")
        where TNode : class, ITreeNode
        where T : class?, TCompare?
        => node.Cyclic(ref cached, ref value, AttributeFunction.Create(compute), initializer, comparer, attributeName);
    #endregion

    #region RewritableChild overloads
    /// <summary>
    /// Read the value of a rewritable child attribute.
    /// </summary>
    [Inline] // Not always working
    public static TChild RewritableChild<TNode, TChild>(
        this TNode node,
        ref bool cached,
        ref RewritableChild<TChild> child,
        [CallerMemberName] string attributeName = "")
        where TNode : class, ITreeNode
        where TChild : class?, IChildTreeNode<TNode>?
        => node.Cyclic(ref cached, ref child, AttributeFunction.RewritableChild<TNode, TChild>(),
            default(Func<TNode, TChild>), ReferenceEqualityComparer.Instance, attributeName);
    #endregion

    #region Cyclic overloads
    /// <summary>
    /// Read the value of a circular attribute.
    /// </summary>
    internal static T Cyclic<TNode, T, TCyclic, TFunc, TCompare>(
        this TNode node,
        ref bool cached,
        ref TCyclic value,
        TFunc func,
        [RequireStaticDelegate] Func<TNode, T>? initializer,
        IEqualityComparer<TCompare> comparer,
        [CallerMemberName] string attributeName = "")
        where TNode : class, ITreeNode
        where T : class?, TCompare?
        where TCyclic : struct, ICyclic<T>
        where TFunc : ICyclicAttributeFunction<TNode, T>
    {
        if (string.IsNullOrEmpty(attributeName))
            throw new ArgumentException("The attribute name must be provided.", nameof(attributeName));
        return Cyclic(node, ref cached, ref value, func, initializer, comparer, new AttributeId(node, attributeName));
    }

    /// <summary>
    /// Read the value of a circular attribute.
    /// </summary>
    internal static T Cyclic<TNode, T, TCyclic, TFunc, TCompare>(
        TNode node,
        ref bool cached,
        ref TCyclic value,
        TFunc func,
        [RequireStaticDelegate] Func<TNode, T>? initializer,
        IEqualityComparer<TCompare> comparer,
        AttributeId attributeId,
        object? cachedOwner = null)
        where TNode : class, ITreeNode
        where T : class?, TCompare?
        where TCyclic : struct, ICyclic<T>
        where TFunc : ICyclicAttributeFunction<TNode, T>
    {
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

        if (TCyclic.IsFinalValue(current))
        {
            Volatile.Write(ref cached, true);
            return current;
        }

        var threadState = ThreadState();
        if (threadState.CheckInStackAndUpdateLowLink(attributeId))
            return current;

        var cachedRef = InteriorRef.Create(cachedOwner ?? node, ref cached);
        using var attributeScope = threadState.VisitCyclic(attributeId, cachedRef, TCyclic.IsRewritableAttribute, current as IChildTreeNode);
        do
        {
            attributeScope.NextIteration();
            if (TCyclic.IsRewritableAttribute)
            {
                // Rewrites inherently depend on themselves. The low link should be at least that of the
                // rewrite. The compute function on the next line will actually use `current`.
                attributeScope.RewritableDependsOnSelf();
            }
            var next = func.Compute(node, current); // may throw
            if (comparer.Equals(current, next)) // may throw
                // current == next, so use old value to avoid duplicate objects referenced
                continue;

            // This attribute changed
            attributeScope.MarkChanged();

            var original = value.CompareExchange(next, current);
            if (!ReferenceEquals(original, current))
            {
                // The value was changed by another thread, so use the new value. First though, check
                // whether it is cached and therefore final.
                var isFinal = Volatile.Read(in cached);
                if (isFinal)
                {
                    attributeScope.MarkFinal();
                    attributeScope.Success();
                    // Read again if final to ensure the value is the one that is actually cached
                    return value.UnsafeValue;
                }
                next = original;
            }

            if (TCyclic.IsRewritableAttribute)
            {
                // If the node gets rewritten into one that can't be rewritten, the value is final
                if (TCyclic.IsFinalValue(next))
                {
                    attributeScope.MarkRewritableFinal();
                    Volatile.Write(ref cached, true);
                    attributeScope.Success();
                    // Use next. It is the new value because current hasn't changed yet.
                    return next;
                }
                // The `next` value is now part of the same rewrite context as the `current` was
                // even if another thread set the value and `next` was taken from that.
                attributeScope.AddToRewriteContext(current!, next);
            }

            current = next;
        } while (attributeScope.RootOfChangedComponent);

        // If either the attribute is final or the value is final, then the value can be cached
        if (attributeScope.IsFinal || TCyclic.IsFinalValue(current))
            Volatile.Write(ref cached, true);

        attributeScope.Success();
        return current;
    }
    #endregion
}
