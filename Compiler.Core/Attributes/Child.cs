using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

[StructLayout(LayoutKind.Auto)]
public struct Child<T>
    where T : class, IRewritableChild<T>
{
    private T value;
    // Declared as a uint to ensure Interlocked can be used on it (without any possible overhead)
    private volatile uint state; // Defaults to ChildState.NotSet

    internal Child(T initialValue)
    {
        state = initialValue.MayHaveRewrite ? (uint)ChildState.Initial : (uint)ChildState.Final;
        value = initialValue;
    }

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
                while (true)
                {
                    var newValue = value.Rewrite();
                    if (ReferenceEquals(value, newValue))
                    {
                        // Volatile write ensures the last value write cannot be moved after it
                        state = (uint)ChildState.Final;
                        return newValue;
                    }

                    // Use a volatile write of the value to try to write in progress values in such
                    // a way that they can be read by other threads.
                    Volatile.Write(ref value, newValue);
                }
            case ChildState.InProgress:
                if (readFinal)
                    throw new InvalidOperationException("Cannot read final value while the child is being computed.");
                // Use a volatile read of the value to try to read in progress values.
                return Volatile.Read(ref value);
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
    [return: NotNullIfNotNull(nameof(child))]
    public static TChild? Attach<TParent, TChild>(TParent parent, TChild? child)
        where TChild : class, IChild<TParent>
    {
        if (child == null)
            return child;

        if (child.MayHaveRewrite)
            throw new NotSupportedException(ChildMayHaveRewriteMessage(child));
        child.AttachParent(parent);
        return child;
    }

    public static Child<TChild> Create<TParent, TChild>(TParent parent, TChild initialValue)
        where TChild : class, IChild<TChild, TParent>
    {
        initialValue.AttachParent(parent);
        return new Child<TChild>(initialValue);
    }

    [return: NotNullIfNotNull(nameof(initialValue))]
    public static Child<TChild>? CreateOptional<TParent, TChild>(TParent parent, TChild? initialValue)
        where TChild : class, IChild<TChild, TParent>
    {
        if (initialValue is null)
            return null;
        initialValue.AttachParent(parent);
        return new Child<TChild>(initialValue);
    }

    public static string InheritFailedMessage<TChild>(string attribute, TChild caller, TChild child)
        where TChild : IChild
        => $"{attribute} not implemented for child node type {child.GetType().GetFriendlyName()} "
           + $"when accessed through caller {caller.GetType().GetFriendlyName()}.";

    public static string ParentMissingMessage(IChild child)
        => $"Parent of {child.GetType().GetFriendlyName()} is not set.";

    internal static string ChildMayHaveRewriteMessage(IChild child)
        => $"{child.GetType().GetFriendlyName()} may have rewrites and cannot be used as a fixed child.";
}
