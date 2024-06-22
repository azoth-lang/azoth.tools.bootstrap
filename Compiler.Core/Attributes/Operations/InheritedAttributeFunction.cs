using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

public readonly struct InheritedAttributeFunction<TNode, T> : IAttributeFunction<TNode, T>
{
    private readonly Func<TNode, IInheritanceContext, T> compute;

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InheritedAttributeFunction(Func<TNode, IInheritanceContext, T> compute)
    {
        this.compute = compute;
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Compute(TNode node, IInheritanceContext ctx) => compute(node, ctx);
}
