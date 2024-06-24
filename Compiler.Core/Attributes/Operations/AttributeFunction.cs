using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

public static class AttributeFunction
{
    [Inline] // Not always working
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AttributeFunction<TNode, T> Create<TNode, T>(Func<TNode, T> compute)
        => new(compute);

    [Inline] // Not always working
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static InheritedAttributeFunction<TNode, T> Create<TNode, T>(Func<IInheritanceContext, T> compute)
        => new(compute);

    [Inline] // Not always working
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RewritableAttributeFunction<TNode, T> Rewritable<TNode, T>()
        where TNode : ITreeNode
        where T : IChildTreeNode<TNode>?
        => new();
}

public readonly struct AttributeFunction<TNode, T> : IAttributeFunction<TNode, T>, ICyclicAttributeFunction<TNode, T>
{
    private readonly Func<TNode, T> compute;

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AttributeFunction(Func<TNode, T> compute)
    {
        this.compute = compute;
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Compute(TNode node, IInheritanceContext _) => compute(node);


    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Compute(TNode node, T _) => compute(node);
}
