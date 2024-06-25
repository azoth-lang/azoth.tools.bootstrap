using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;
using Azoth.Tools.Bootstrap.Framework;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

internal class RewritableChildList<TNode, TChild> : IRewritableChildList<TChild>
    where TNode : class, ITreeNode
    where TChild : class, IChildTreeNode<TNode>
{
    private readonly TNode node;
    private readonly string attributeName;
    // Due to RewritableChild<T> being a mutable struct, we use Buffer<T> to force always getting a
    // reference to the struct when accessing it.
    private readonly Buffer<RewritableChild<TChild>> children;
    // Store cached values separately to avoid allocating any more memory than necessary.
    private readonly bool[] cached;

    public IFixedList<TChild> Current { get; }

    internal RewritableChildList(TNode node, string attributeName, IEnumerable<TChild> initialValues)
    {
        this.node = node;
        this.attributeName = attributeName;
        // Use a buffer to avoid allocating any more memory than necessary and because we need to
        // get a reference to the Child<T> struct when accessing it.
        children = initialValues.Select(x => new RewritableChild<TChild>(x)).ToBuffer();
        cached = new bool[children.Count];
        Current = new CurrentList(children);
    }

    public int Count => children.Count;

    public TChild this[int index] => Get(index);

    [Inline]
    private TChild Get(int index)
        => GrammarAttribute.IsCached(in cached[index]) ? children[index].UnsafeValue
        : GrammarAttribute.Cyclic(node, ref cached[index], ref children[index],
            AttributeFunction.RewritableChild<TNode, TChild>(), default(Func<TNode, TChild>),
            ReferenceEqualityComparer.Instance, new(node, attributeName, index), cached);

    public IEnumerator<TChild> GetEnumerator()
    {
        for (int i = 0; i < children.Count; i++)
            yield return Get(i);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private sealed class CurrentList(Buffer<RewritableChild<TChild>> children) : IFixedList<TChild>
    {
        public int Count => children.Count;

        public TChild this[int index] => children[index].UnsafeValue;

        public IEnumerator<TChild> GetEnumerator()
        {
            for (int i = 0; i < children.Count; i++)
                yield return children[i].UnsafeValue;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

internal sealed class RewritableChildList<TNode, TChild, TFinal> : RewritableChildList<TNode, TChild>, IRewritableChildList<TChild, TFinal>
    where TNode : class, ITreeNode
    where TChild : class, IChildTreeNode<TNode>
    where TFinal : class, IChildTreeNode
{
    public IFixedList<TFinal?> Intermediate { get; }

    internal RewritableChildList(TNode node, string attributeName, IEnumerable<TChild> initialValues)
        : base(node, attributeName, initialValues)
    {
        Intermediate = new IntermediateList(this);
    }

    private sealed class IntermediateList(RewritableChildList<TNode, TChild> children) : IFixedList<TFinal?>
    {
        public int Count => children.Count;

        public TFinal? this[int index] => children[index] as TFinal;

        public IEnumerator<TFinal?> GetEnumerator()
        {
            foreach (var child in children)
                yield return child as TFinal;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
