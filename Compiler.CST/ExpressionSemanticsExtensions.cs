using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Core;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public static class ExpressionSemanticsExtensions
{
    public static string Action(this ExpressionSemantics valueSemantics)
    {
        return valueSemantics switch
        {
            ExpressionSemantics.Never => "never",
            ExpressionSemantics.Void => "void",
            ExpressionSemantics.MoveValue => "move",
            ExpressionSemantics.CopyValue => "copy",
            ExpressionSemantics.IsolatedReference => "iso",
            ExpressionSemantics.MutableReference => "mut",
            ExpressionSemantics.ReadOnlyReference => "read",
            ExpressionSemantics.ConstReference => "const",
            ExpressionSemantics.IdReference => "id",
            ExpressionSemantics.CreateReference => "ref",
            _ => throw ExhaustiveMatch.Failed(valueSemantics),
        };
    }

    /// <summary>
    /// Validates that expression semantics have been assigned.
    /// </summary>
    [DebuggerHidden]
    public static ExpressionSemantics Assigned([NotNull] this ExpressionSemantics? semantics)
        => semantics ?? throw new InvalidOperationException("Expression semantics not assigned");
}
