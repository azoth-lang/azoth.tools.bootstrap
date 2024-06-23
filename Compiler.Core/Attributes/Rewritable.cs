using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

public struct Rewritable<T> : ICyclic<T>
    where T : class?, IChild?
{
    private T rawValue;

    public readonly bool IsInitialized => true;

    public readonly T UnsafeValue => rawValue;

    /// <remarks>A rewritable attribute is final if it can't have rewrites or is null.</remarks>
    public bool IsFinal => !rawValue?.MayHaveRewrite ?? true;

    internal Rewritable(T initialValue)
    {
        rawValue = initialValue;
    }

    void ICyclic<T>.Initialize(T _)
        => throw new NotSupportedException($"{nameof(Rewritable<T>)} attributes are always initialized.");

    T ICyclic<T>.CompareExchange(T value, T comparand)
        => Unsafe.As<T>(Interlocked.CompareExchange(ref rawValue!, value, comparand));
}
