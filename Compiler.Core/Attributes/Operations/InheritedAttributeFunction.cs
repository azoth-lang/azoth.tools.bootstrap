using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

public readonly struct InheritedAttributeFunction<TNode, T> : IAttributeFunction<TNode, T>
{
    private readonly Func<IInheritanceContext, T> compute;

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InheritedAttributeFunction(Func<IInheritanceContext, T> compute)
    {
        this.compute = compute;
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Compute(TNode _, IInheritanceContext ctx) => compute(ctx);
}
