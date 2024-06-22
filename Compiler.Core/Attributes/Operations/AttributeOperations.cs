using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Framework;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

public static class AttributeOperations
{
    [Inline] // Not always working
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SyntheticReferenceAttributeOperations<TNode, T> Synthetic<TNode, T>(
        Func<TNode, T> compute,
        IEqualityComparer<T> comparer)
        where T : class?
        => new(compute, comparer);

    [Inline] // Not always working
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SyntheticReferenceAttributeOperations<TNode, T> Synthetic<TNode, T>(
        Func<TNode, T> compute)
        where T : class?
        => new(compute, StrictEqualityComparer<T>.Instance);
}
