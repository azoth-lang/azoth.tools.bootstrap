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
    public static SyntheticAttributeFunction<TNode, T> Create<TNode, T>(Func<TNode, T> compute)
        => new(compute);

    [Inline] // Not always working
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static InheritedAttributeFunction<TNode, T> Create<TNode, T>(Func<IInheritanceContext, T> compute)
        => new(compute);
}
