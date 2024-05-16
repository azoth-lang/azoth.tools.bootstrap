using System;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

public struct Child<T>
    where T : class
{
    private T? initialValue;

    public Child(T initialValue)
    {
        this.initialValue = initialValue;
    }

    public T Value => initialValue!;
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
        where TChild : class, IChild<TParent>
    {
        initialValue.AttachParent(parent);
        return new Child<TChild>(initialValue);
    }

    [return: NotNullIfNotNull(nameof(initialValue))]
    public static Child<TChild>? CreateOptional<TParent, TChild>(TParent parent, TChild? initialValue)
        where TChild : class, IChild<TParent>
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
