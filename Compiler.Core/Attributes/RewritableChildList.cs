using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;
using Azoth.Tools.Bootstrap.Framework;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

[DebuggerStepThrough]
internal abstract class RewritableChildList<T> : IFixedList<T>
{
    private int hashCode;

    public abstract int Count { get; }
    public bool IsEmpty => Count == 0;

    public abstract T this[int index] { get; }

    public abstract IEnumerator<T> GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #region Equality
    public override bool Equals(object? obj) => IFixedList<T>.Equals(this, obj);

    public sealed override int GetHashCode()
        => hashCode != 0 ? hashCode : hashCode = IFixedList<T>.ComputeHashCode(this);
    #endregion
}

[DebuggerStepThrough]
internal class RewritableChildList<TNode, TChild> : RewritableChildList<TChild>, IRewritableChildList<TChild>
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

    public override int Count => children.Count;

    public override TChild this[int index] => Get(index);

    [Inline]
    private TChild Get(int index)
        => GrammarAttribute.IsCached(in cached[index]) ? children[index].UnsafeValue
        : GrammarAttribute.Cyclic(node, ref cached[index], ref children[index],
            AttributeFunction.RewritableChild<TNode, TChild>(), default(Func<TNode, TChild>),
            ReferenceEqualityComparer.Instance, new(node, attributeName, index), cached);

    public override IEnumerator<TChild> GetEnumerator()
    {
        for (int i = 0; i < children.Count; i++)
            yield return Get(i);
    }

    private sealed class CurrentList(Buffer<RewritableChild<TChild>> children) : RewritableChildList<TChild>, IFixedList<TChild>
    {
        public override int Count => children.Count;

        public override TChild this[int index] => children[index].UnsafeValue;

        public override IEnumerator<TChild> GetEnumerator()
        {
            for (int i = 0; i < children.Count; i++)
                yield return children[i].UnsafeValue;
        }
    }
}

[DebuggerStepThrough]
internal sealed class RewritableChildList<TNode, TChild, TFinal> : RewritableChildList<TNode, TChild>, IRewritableChildList<TChild, TFinal>
    where TNode : class, ITreeNode
    where TChild : class, IChildTreeNode<TNode>
    where TFinal : class, IChildTreeNode
{
    public IFixedList<TFinal?> AsFinalType { get; }

    internal RewritableChildList(TNode node, string attributeName, IEnumerable<TChild> initialValues)
        : base(node, attributeName, initialValues)
    {
        AsFinalType = new IntermediateList(this);
    }

    private sealed class IntermediateList(RewritableChildList<TNode, TChild> children) : RewritableChildList<TFinal?>, IFixedList<TFinal?>
    {
        public override int Count => children.Count;

        public override TFinal? this[int index] => children[index] as TFinal;

        public override IEnumerator<TFinal?> GetEnumerator()
        {
            foreach (var child in children)
                yield return child as TFinal;
        }
    }
}
