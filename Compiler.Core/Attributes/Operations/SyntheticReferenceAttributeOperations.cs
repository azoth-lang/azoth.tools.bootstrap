using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

public readonly struct SyntheticReferenceAttributeOperations<TNode, T> : IAttributeOperations<TNode, T>
    where T : class?
{
    private readonly Func<TNode, T> compute;
    private readonly IEqualityComparer<T> comparer;

    internal SyntheticReferenceAttributeOperations(Func<TNode, T> compute, IEqualityComparer<T> comparer)
    {
        this.compute = compute;
        this.comparer = comparer;
    }

    internal SyntheticReferenceAttributeOperations(Func<TNode, T> compute)
        : this(compute, StrictEqualityComparer<T>.Instance)
    {
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Compute(TNode node, IInheritanceContext _) => compute(node);

    public bool CompareExchange(ref T location, T value, T? comparand, out T previous)
    {
        previous = Interlocked.CompareExchange(ref location!, value, comparand);
        return ReferenceEquals(previous, comparand);
    }

    public bool Equals(T? x, T? y) => comparer.Equals(x, y);

    public int GetHashCode([DisallowNull] T obj) => comparer.GetHashCode(obj);
}
