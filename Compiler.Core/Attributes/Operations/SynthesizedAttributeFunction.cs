using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

[StructLayout(LayoutKind.Auto)]
internal readonly struct SynthesizedAttributeFunction<TNode, T> : IAttributeFunction<TNode, T>, ICyclicAttributeFunction<TNode, T>
{
    private readonly Func<TNode, T> compute;

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SynthesizedAttributeFunction(Func<TNode, T> compute)
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
