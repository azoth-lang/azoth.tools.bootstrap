using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

public sealed class ChildList<T> : IFixedList<T>
    where T : class, IChildTreeNode
{
    /// <summary>
    /// Create a list of potentially rewritable children.
    /// </summary>
    public static IRewritableChildList<TChild, T> Create<TParent, TChild>(
        TParent parent,
        string attributeName,
        IEnumerable<TChild> initialValues)
        where TParent : class, ITreeNode
        where TChild : class, IChildTreeNode<TParent>
    {
        var children = new RewritableChildList<TParent, TChild, T>(parent, attributeName, initialValues);
        foreach (var child in children.Current)
            child.SetParent(parent);
        return children;
    }

    // Due to Child<T> being a mutable struct, we use Buffer<T> to force always getting a reference
    // to the struct when accessing it.
    private readonly Buffer<Child<T>> children;

    internal ChildList(IEnumerable<T> initialValues)
    {
        // Use a buffer to avoid allocating any more memory than necessary and because we need to
        // get a reference to the Child<T> struct when accessing it.
        children = initialValues.Select(x => new Child<T>(x)).ToBuffer();
    }

    public int Count => children.Count;

    public T this[int index]
        => children[index].Value;

    public IEnumerable<T> Final
    {
        get
        {
            for (int i = 0; i < children.Count; i++)
                yield return children[i].FinalValue;
        }
    }

    public IEnumerable<T> Current
    {
        get
        {
            for (int i = 0; i < children.Count; i++)
                yield return children[i].CurrentValue;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < children.Count; i++)
            yield return children[i].FinalValue;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int? IndexOfCurrent(T item)
    {
        for (int i = 0; i < children.Count; i++)
            if (children[i].CurrentValue == item)
                return i;

        return null;
    }

    public T FinalAt(int index) => children[index].FinalValue;
}

public static class ChildList
{
    /// <summary>
    /// Create a list of potentially rewritable children.
    /// </summary>
    public static ChildList<TChild> CreateLegacy<TParent, TChild>(TParent parent, IEnumerable<TChild> initialValues)
        where TParent : ITreeNode
        where TChild : class, IChildTreeNode<TParent>
    {
        var children = new ChildList<TChild>(initialValues);
        foreach (var child in children.Current)
            child.SetParent(parent);
        return children;
    }

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
