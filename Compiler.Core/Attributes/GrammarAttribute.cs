using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    [Inline]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static AttributeGrammarThreadState ThreadState()
        // Do not need to use LazyInitializer here because this is thread static
        => _threadStateStorage ??= new();

    /// <summary>
    /// Safely check whether the attribute has been cached. If it has been, then it is safe to
    /// simply read the attribute value from the backing field.
    /// </summary>
    [Inline(export: true)]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCached(in bool cached) => Volatile.Read(in cached);

    /// <summary>
    /// Get the inheritance context for the current thread.
    /// </summary>
    /// <remarks>This should only be used for nodes that directly expose a function that calls the
    /// inherited member.</remarks>
    [Inline]
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
    [DebuggerStepThrough]
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
        where TNode : class, ITreeNode
        where TFunc : struct, IAttributeFunction<TNode, T>
        where T : struct
        => node.NonCircular<TNode, T, TFunc, ValueOperations<T>, AttributeLock>(ref cached, ref value,
            func, comparer, ref syncLock, attributeName);

    [Inline(export: true)]
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
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Synthetic<TNode, T>(
        this TNode node,
        ref bool cached,
        ref T? value,
        Func<TNode, T> compute,
        [CallerMemberName] string attributeName = "")
        where TNode : class, ITreeNode
        where T : class?
        => node.NonCircular(ref cached, ref value, AttributeFunction.Create(compute),
            EqualityComparer<T>.Default, attributeName);

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
        where TNode : class, ITreeNode
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
        where TNode : class, ITreeNode
        where T : struct
        => node.NonCircular(ref cached, ref value, AttributeFunction.Create(compute),
            EqualityComparer<T>.Default, ref syncLock, attributeName);

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
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Inherited<TNode, T>(
        this TNode node,
        ref bool cached,
        ref T? value,
        Func<IInheritanceContext, T> compute,
        [CallerMemberName] string attributeName = "")
        where TNode : class, ITreeNode
        where T : class?
        => node.NonCircular(ref cached, ref value, AttributeFunction.Create<TNode, T>(compute),
            EqualityComparer<T>.Default, attributeName);

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
        where TNode : class, ITreeNode
        where T : class?
        => node.NonCircular(ref cached, ref value, AttributeFunction.Create<TNode, T>(compute), comparer,
            attributeName);

    /// <summary>
    /// Read the value of a non-circular inherited attribute that is <see cref="IEquatable{T}"/>.
    /// </summary>
    [Inline]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Inherited<TNode, T>(
        this TNode node,
        ref bool cached,
        ref T value,
        ref AttributeLock syncLock,
        Func<IInheritanceContext, T> compute,
        [CallerMemberName] string attributeName = "")
        where TNode : class, ITreeNode
        where T : struct
        => node.NonCircular(ref cached, ref value, AttributeFunction.Create<TNode, T>(compute),
            EqualityComparer<T>.Default, ref syncLock, attributeName);

    /// <summary>
    /// Read the value of a non-circular inherited attribute that is <see cref="IEquatable{T}"/>.
    /// </summary>
    [Inline]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Inherited<TNode, T>(
        this TNode node,
        ref bool cached,
        ref T? value,
        ref AttributeLock syncLock,
        Func<IInheritanceContext, T?> compute,
        [CallerMemberName] string attributeName = "")
        where TNode : class, ITreeNode
        where T : struct
        => node.NonCircular(ref cached, ref value, AttributeFunction.Create<TNode, T?>(compute),
            EqualityComparer<T?>.Default, ref syncLock, attributeName);
    #endregion

    #region Circular overloads
    /// <summary>
    /// Read the value of a circular attribute that already has an initial value and is
    /// <see cref="IEquatable{T}"/>.
    /// </summary>
    [Inline] // Not always working
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Circular<TNode, T>(
        this TNode node,
        ref bool cached,
        ref Circular<T> value,
        Func<TNode, T> compute,
        [CallerMemberName] string attributeName = "")
        where TNode : class, ITreeNode
        where T : class?
        => node.Cyclic(ref cached, ref value, AttributeFunction.Create(compute), default(Func<TNode, T>),
            EqualityComparer<T>.Default, attributeName);

    /// <summary>
    /// Read the value of a circular attribute that already has an initial value.
    /// </summary>
    [Inline] // Not always working
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Circular<TNode, T, TCompare>(
        this TNode node,
        ref bool cached,
        ref Circular<T> value,
        Func<TNode, T> compute,
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
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Circular<TNode, T>(
        this TNode node,
        ref bool cached,
        ref Circular<T> value,
        Func<TNode, T> compute,
        Func<TNode, T> initializer,
        [CallerMemberName] string attributeName = "")
        where TNode : class, ITreeNode
        where T : class?
        => node.Cyclic(ref cached, ref value, AttributeFunction.Create(compute), initializer,
            EqualityComparer<T>.Default, attributeName);

    /// <summary>
    /// Read the value of a circular attribute.
    /// </summary>
    [Inline] // Not always working
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Circular<TNode, T, TCompare>(
        this TNode node,
        ref bool cached,
        ref Circular<T> value,
        Func<TNode, T> compute,
        Func<TNode, T> initializer,
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
    [DebuggerStepThrough]
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
    [DebuggerStepThrough]
    internal static T Cyclic<TNode, T, TCyclic, TFunc, TCompare>(
        this TNode node,
        ref bool cached,
        ref TCyclic value,
        TFunc func,
        Func<TNode, T>? initializer,
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
    //[DebuggerStepThrough]
    internal static T Cyclic<TNode, T, TCyclic, TFunc, TCompare>(
        TNode node,
        ref bool cached,
        ref TCyclic value,
        TFunc func,
        Func<TNode, T>? initializer,
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
                attributeScope.AddToRewriteContext(current!, next);

            current = next;

            if (TCyclic.IsFinalValue(current))
                // TODO there are issues with this. This node may be final, but if dependencies
                // changed or were cycles, then the parent needs to know that
                attributeScope.MarkFinal();

        } while (attributeScope.RootOfChangedComponent);

        if (attributeScope.IsFinal)
            Volatile.Write(ref cached, true);

        attributeScope.Success();
        return current;
    }
    #endregion
}
