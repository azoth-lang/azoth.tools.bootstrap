using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

[DebuggerStepThrough]
public struct RewritableChild<T> : ICyclic<T>
    where T : class?, IChildTreeNode?
{
    public static bool IsRewritableAttribute => true;

    /// <remarks>A rewritable attribute is final if it can't have rewrites or is null.</remarks>
    public static bool IsFinalValue(T value) => !value?.MayHaveRewrite ?? true;

    private T rawValue;

    public readonly bool IsInitialized => true;

    public readonly T UnsafeValue => rawValue;

    internal RewritableChild(T initialValue)
    {
        rawValue = initialValue;
    }

    void ICyclic<T>.Initialize(T _)
        => throw new NotSupportedException($"{nameof(RewritableChild<T>)} attributes are always initialized.");

    T ICyclic<T>.CompareExchange(T value, T comparand)
        => Unsafe.As<T>(Interlocked.CompareExchange(ref rawValue!, value, comparand));
}
