using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class CapabilityExpressionsAspect
{
    public static partial IAmbiguousExpressionNode? AmbiguousMoveExpression_Rewrite_Variable(IAmbiguousMoveExpressionNode node);
    public static partial IAmbiguousExpressionNode? AmbiguousMoveExpression_Rewrite_Value(IAmbiguousMoveExpressionNode node);
    public static partial IAmbiguousExpressionNode? AmbiguousFreezeExpression_Rewrite_Variable(IAmbiguousFreezeExpressionNode node);
    public static partial IAmbiguousExpressionNode? AmbiguousFreezeExpression_Rewrite_Value(IAmbiguousFreezeExpressionNode node);
}
