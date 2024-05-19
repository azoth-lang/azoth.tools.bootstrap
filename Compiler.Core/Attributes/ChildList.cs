using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

public sealed class ChildList<T> : IFixedList<T>
    where T : class, IChild
{
    private readonly Child<T>[] children;

    internal ChildList(IEnumerable<T> initialValues)
    {
        // Use an array to avoid allocating any more memory than necessary and because we need to
        // get a reference to the Child<T> struct when accessing it.
        children = initialValues.Select(x => new Child<T>(x)).ToArray();
    }

    public int Count => children.Length;

    public T this[int index]
        // Due to Child<T> being a mutable struct, we must use GetRefToChild() to get a
        // reference that can mutate the original struct.
        => GetRefToChild(index).Value;

    public IEnumerable<T> Final
    {
        get
        {
            // Due to Child<T> being a mutable struct, we must use GetRefToChild() to get a
            // reference that can mutate the original struct.
            for (int i = 0; i < children.Length; i++)
                yield return GetRefToChild(i).FinalValue;
        }
    }

    internal IEnumerable<T> Current
    {
        get
        {
            // Due to Child<T> being a mutable struct, we must use GetRefToChild() to get a
            // reference that can mutate the original struct. This is true even for CurrentValue
            // because it does a volatile read.
            for (int i = 0; i < children.Length; i++)
                yield return GetRefToChild(i).CurrentValue;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        // Due to Child<T> being a mutable struct, we must use GetRefToChild() to get a
        // reference that can mutate the original struct.
        for (int i = 0; i < children.Length; i++)
            yield return GetRefToChild(i).FinalValue;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <remarks>Because <see cref="Child{T}"/> is a mutable struct, we must get a reference to it
    /// when accessing it from the array. </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref Child<T> GetRefToChild(int index) => ref children[index];
}

public static class ChildList
{
    /// <summary>
    /// Create a list of potentially rewritable children.
    /// </summary>
    public static ChildList<TChild> Create<TParent, TChild>(TParent parent, IEnumerable<TChild> initialValues)
        where TChild : class, IChild<TParent>
    {
        var children = new ChildList<TChild>(initialValues);
        foreach (var child in children.Current)
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
