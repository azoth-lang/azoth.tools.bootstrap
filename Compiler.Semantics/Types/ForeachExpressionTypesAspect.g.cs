using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Flow;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class ForeachExpressionTypesAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType ForeachExpression_Type(IForeachExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState ForeachExpression_FlowStateAfter(IForeachExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void ForeachExpression_Contribute_Diagnostics(IForeachExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeNonVoidType ForeachExpression_IteratorType(IForeachExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeNonVoidType ForeachExpression_IteratedType(IForeachExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState ForeachExpression_FlowStateBeforeBlock(IForeachExpressionNode node);
}
