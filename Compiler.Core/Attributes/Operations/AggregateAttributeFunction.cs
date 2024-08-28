using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

internal readonly struct AggregateAttributeFunction<TNode, T> : IAttributeFunction<TNode, T>
{
    private readonly Func<T> compute;

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AggregateAttributeFunction(Func<T> compute)
    {
        this.compute = compute;
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Compute(TNode node, IInheritanceContext ctx) => compute();
}
