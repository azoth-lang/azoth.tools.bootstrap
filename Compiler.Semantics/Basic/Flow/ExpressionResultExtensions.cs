using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

public static class ExpressionResultExtensions
{
    /// <summary>
    /// Validates that a type as been assigned.
    /// </summary>
    [DebuggerHidden]
    public static ExpressionResult Assigned([NotNull] this ExpressionResult? result)
        => result ?? throw new InvalidOperationException("ExpressionResult not assigned.");
}
