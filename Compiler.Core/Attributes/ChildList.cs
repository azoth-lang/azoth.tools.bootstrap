using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

public static class ChildList
{
    /// <summary>
    /// Create a list of potentially rewritable children.
    /// </summary>
    public static IRewritableChildList<TChild> Create<TParent, TChild>(TParent parent, string attributeName, IEnumerable<TChild> initialValues)
        where TParent : class, ITreeNode
        where TChild : class, IChildTreeNode<TParent>
    {
        var children = new RewritableChildList<TParent, TChild>(parent, attributeName, initialValues);
        foreach (var child in children.Current)
            child.SetParent(parent);
        return children;
    }

    /// <summary>
    /// Attach a list of children that does not support rewriting.
    /// </summary>
    public static IFixedList<TChild> Attach<TParent, TChild>(TParent parent, IEnumerable<TChild> children)
        where TParent : ITreeNode
        where TChild : class, IChildTreeNode<TParent>
    {
        var childList = children.ToFixedList();
        foreach (var child in childList)
            Child.Attach(parent, child);
        return childList;
    }
}

public static class ChildList<TFinal>
    where TFinal : class, IChildTreeNode
{
    /// <summary>
    /// Create a list of potentially rewritable children.
    /// </summary>
    public static IRewritableChildList<TChild, TFinal> Create<TParent, TChild>(
        TParent parent,
        string attributeName,
        IEnumerable<TChild> initialValues)
        where TParent : class, ITreeNode
        where TChild : class, IChildTreeNode<TParent>
    {
        var children = new RewritableChildList<TParent, TChild, TFinal>(parent, attributeName, initialValues);
        foreach (var child in children.Current) child.SetParent(parent);
        return children;
    }
}
