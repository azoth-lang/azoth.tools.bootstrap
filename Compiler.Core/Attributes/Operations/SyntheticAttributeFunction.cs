using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

public readonly struct SyntheticAttributeFunction<TNode, T> : IAttributeFunction<TNode, T>
{
    private readonly Func<TNode, T> compute;

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SyntheticAttributeFunction(Func<TNode, T> compute)
    {
        this.compute = compute;
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Compute(TNode node, IInheritanceContext _)
        => compute(node);
}
