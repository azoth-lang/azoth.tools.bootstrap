using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

[DebuggerStepThrough]
[StructLayout(LayoutKind.Auto)]
public struct RewritableChild<T> : ICyclic<T>
    where T : class?, IChildTreeNode?
{
    public static bool IsRewritableAttribute => true;

    /// <remarks>There are two cases when a rewritable attribute can be known to be final. First,
    /// if it is null or can't have rewrites. Second, if it has been marked as being in the final
    /// tree. In the latter case, the rewrite contexts are sometimes able to determine a rewrite to
    /// be final and mark is as <see cref="ITreeNode.InFinalTree"/> even though otherwise it would
    /// not be clear that the rewrite is final.</remarks>
    // TODO understand and validate the second case. Is there not some code that could hold a
    // reference into an incomplete tree and then visit it after stepping out of the rewrite context
    // and thereby incorrectly mark the referenced tree as InFinalTree when it shouldn't be?
    public static bool IsFinalValue(T value)
    {
        if (value is null) return true;
        return !value.MayHaveRewrite || value.InFinalTree;
    }

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
