using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class ForeachExpressionTypeAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState ForeachExpression_FlowStateAfter(IForeachExpressionNode node);
}
