using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

[StructLayout(LayoutKind.Auto)]
public struct Child<T>
    where T : class?, IChildTreeNode?
{
    private T value;
    // Declared as an uint to ensure Interlocked can be used on it (without any possible overhead)
    private volatile uint state; // Defaults to ChildState.NotSet

    internal Child(T initialValue)
    {
        value = initialValue;
        // Volatile write ensures the value write cannot be moved after it
        state = initialValue is not null && initialValue.MayHaveRewrite ? (uint)ChildState.Initial : (uint)ChildState.Final;
    }

    /// <summary>
    /// Get the current value without attempting any rewrites.
    /// </summary>
    /// <remarks>This isn't marked <see langword="readonly"/> because it does a volatile read so we
    /// don't want people to think it is ok to copy the struct before accessing it.</remarks>
    public T CurrentValue => Volatile.Read(in value);

    public T Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Read(false);
    }

    public T FinalValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Read(true);
    }

    private T Read(bool readFinal)
    {
        // Volatile read ensures the value read cannot be moved before it
        var currentState = (ChildState)state;
        switch (currentState)
        {
            default:
                throw ExhaustiveMatch.Failed(currentState);
            case ChildState.NotSet:
                throw new StructNotInitializedException(GetType());
            case ChildState.Initial:
                // Try to acquire the right to compute the value
                currentState = (ChildState)Interlocked.CompareExchange(ref state, (uint)ChildState.InProgress, (uint)ChildState.Initial);
                if (currentState != ChildState.Initial)
                    // This is unlikely to happen, so just recurse to easily handle the changed state
                    return Read(readFinal);

                // We now have the right to compute the value
                while (value!.MayHaveRewrite)
                {
                    var newValue = value!.Rewrite();
                    // Null is the preferred way to signal no change, but returning the same value works too
                    if (newValue is null || ReferenceEquals(value, newValue))
                        break;

                    // Use a volatile write of the value to try to write in progress values in such
                    // a way that they can be read by other threads.
                    Volatile.Write(ref value, (T)newValue);
                }

                // Volatile write ensures the last value write cannot be moved after it
                state = (uint)ChildState.Final;
                return value;
            case ChildState.InProgress:
                if (readFinal)
                    throw new InvalidOperationException("Cannot read final value while the child is being rewritten.");
                // If a rewrite is in progress, just return the current value so that the rewrite
                // can proceed based on the current tree state.
                // Use a volatile read of the value to try to read in progress values.
                return Volatile.Read(in value);
            case ChildState.Final:
                return value;
        }
    }
}

public static class Child
{
    /// <summary>
    /// Attach a child that does not support rewriting.
    /// </summary>
    public static TChild Attach<TParent, TChild>(TParent parent, TChild child)
        where TParent : ITreeNode
        where TChild : class?, IChildTreeNode<TParent>?
    {
        if (child is null)
            return child;

        if (child.MayHaveRewrite)
            throw new NotSupportedException(ChildMayHaveRewriteMessage(child));
        child.SetParent(parent);
        return child;
    }

    /// <summary>
    /// Attach a child that may support rewriting.
    /// </summary>
    public static Rewritable<TChild> CreateRewritable<TParent, TChild>(TParent parent, TChild initialValue)
        where TParent : ITreeNode
        where TChild : class?, IChildTreeNode<TParent>?
    {
        initialValue?.SetParent(parent);
        return new(initialValue);
    }

    /// <summary>
    /// Attach a child that is the result of rewriting and hence may support rewriting.
    /// </summary>
    public static TChild AttachRewritten<TParent, TChild>(TParent parent, TChild child)
        where TParent : ITreeNode
        where TChild : class?, IChildTreeNode<TParent>?
    {
        if (child is null)
            return child;

        child.SetParent(parent);
        return child;
    }

    public static Child<TChild> Create<TParent, TChild>(TParent parent, TChild initialValue)
        where TParent : ITreeNode
        where TChild : class?, IChildTreeNode<TParent>?
    {
        initialValue?.SetParent(parent);
        return new(initialValue);
    }

    // TODO replace with factory for exception
    public static string InheritFailedMessage<TChild>(string attribute, TChild child, TChild descendant)
        where TChild : IChildTreeNode
        => $"{attribute} not implemented for descendant node type {descendant.GetType().GetFriendlyName()} "
           + $"when accessed through child of type {child.GetType().GetFriendlyName()}.";

    // TODO replace with factory for exception
    public static string ParentMissingMessage(IChildTreeNode child)
        => $"Parent of {child.GetType().GetFriendlyName()} is not set.";

    public static NotSupportedException RewriteNotSupported(IChildTreeNode child)
        => new($"Rewrite of {child.GetType().GetFriendlyName()} is not supported.");

    private static string ChildMayHaveRewriteMessage(IChildTreeNode child)
        => $"{child.GetType().GetFriendlyName()} may have rewrites and cannot be used as a fixed child.";

    public static NotImplementedException PreviousFailed(string attribute, IChildTreeNode before)
        => new($"Previous {attribute} of {before.GetType().GetFriendlyName()} not implemented.");
}
