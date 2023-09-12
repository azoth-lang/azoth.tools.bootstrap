using Azoth.Tools.Bootstrap.Compiler.Core;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public static class TypeSemanticsExtensions
{
    public static ExpressionSemantics ToExpressionSemantics(
        this TypeSemantics semantics,
        ExpressionSemantics referenceSemantics)
    {
        return semantics switch
        {
            TypeSemantics.CopyValue => ExpressionSemantics.CopyValue,
            TypeSemantics.MoveValue => ExpressionSemantics.MoveValue,
            TypeSemantics.Never => ExpressionSemantics.Never,
            TypeSemantics.Void => ExpressionSemantics.Void,
            TypeSemantics.Reference => referenceSemantics,
            _ => throw ExhaustiveMatch.Failed(semantics),
        };
    }
}
