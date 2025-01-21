using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class CapabilityExpressionsAspect
{
    public static partial IMoveVariableExpressionNode? AmbiguousMoveExpression_ReplaceWith_MoveVariableExpression(IAmbiguousMoveExpressionNode node);
    public static partial IMoveValueExpressionNode? AmbiguousMoveExpression_ReplaceWith_MoveValueExpression(IAmbiguousMoveExpressionNode node);
    public static partial IAmbiguousExpressionNode? AmbiguousFreezeExpression_Rewrite_Variable(IAmbiguousFreezeExpressionNode node);
    public static partial IAmbiguousExpressionNode? AmbiguousFreezeExpression_Rewrite_Value(IAmbiguousFreezeExpressionNode node);
}
