using System;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

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
    public static RewritableChild<TChild> Create<TParent, TChild>(TParent parent, TChild initialValue)
        where TParent : ITreeNode
        where TChild : class?, IChildTreeNode<TParent>?
    {
        initialValue?.SetParent(parent);
        return new(initialValue);
    }

    public static NotImplementedException InheritFailed<TChild>(string attribute, TChild child, TChild descendant)
        where TChild : IChildTreeNode
        => new($"{attribute} not implemented for descendant node type {descendant.GetType().GetFriendlyName()} "
               + $"when accessed through child of type {child.GetType().GetFriendlyName()}.");

    public static InvalidOperationException ParentMissing(IChildTreeNode child)
        => new($"Parent of {child.GetType().GetFriendlyName()} is not set.");

    public static NotSupportedException RewriteNotSupported(IChildTreeNode child)
        => new($"Rewrite of {child.GetType().GetFriendlyName()} is not supported.");

    private static string ChildMayHaveRewriteMessage(IChildTreeNode child)
        => $"{child.GetType().GetFriendlyName()} may have rewrites and cannot be used as a fixed child.";

    public static NotImplementedException PreviousFailed(string attribute, IChildTreeNode before)
        => new($"Previous {attribute} of {before.GetType().GetFriendlyName()} not implemented.");
}
