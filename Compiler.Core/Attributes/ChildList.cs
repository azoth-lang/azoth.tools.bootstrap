using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

public sealed class ChildList<T> : IReadOnlyList<T>
    where T : class, IRewritableChild<T>
{
    private readonly IReadOnlyList<Child<T>> children;

    internal ChildList(IEnumerable<T> initialValues)
    {
        // Don't use `AsReadOnly` because FixedList<T> is already a wrapper. Use `ToArray` to
        // avoid allocating any more memory than necessary.
        children = initialValues.Select(x => new Child<T>(x)).ToArray();
    }

    public int Count => children.Count;

    public T this[int index] => children[index].Value;

    public IEnumerator<T> GetEnumerator() => children.Select(x => x.Value).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static class ChildList
{
    /// <summary>
    /// Create a list of potentially rewritable children.
    /// </summary>
    public static ChildList<TChild> Create<TParent, TChild>(TParent parent, IEnumerable<TChild> initialValues)
        where TChild : class, IChild<TChild, TParent>
    {
        var children = new ChildList<TChild>(initialValues);
        foreach (var child in children)
            child.AttachParent(parent);
        return children;
    }

    /// <summary>
    /// Attach a list of children that does not support rewriting.
    /// </summary>
    public static IFixedList<TChild> Attach<TParent, TChild>(TParent parent, IEnumerable<TChild> children)
        where TChild : class, IChild<TParent>
    {
        var childList = children.ToFixedList();
        foreach (var child in childList)
            Child.Attach(parent, child);
        return childList;
    }
}
