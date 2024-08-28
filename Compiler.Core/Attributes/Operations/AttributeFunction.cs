using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

internal static class AttributeFunction
{
    [Inline]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SynthesizedAttributeFunction<TNode, T> Create<TNode, T>(Func<TNode, T> compute)
        => new(compute);

    [Inline]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static InheritedAttributeFunction<TNode, T> Create<TNode, T>(Func<IInheritanceContext, T> compute)
        => new(compute);

    [Inline]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AggregateAttributeFunction<TNode, T> Create<TNode, T>(Func<T> compute)
        => new(compute);

    [Inline]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RewritableChildAttributeFunction<TNode, T> RewritableChild<TNode, T>()
        where TNode : ITreeNode
        where T : IChildTreeNode<TNode>?
        => new();
}
