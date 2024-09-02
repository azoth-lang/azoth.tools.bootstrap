using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class ExpressionTypesAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState SelfParameter_FlowStateAfter(ISelfParameterNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState LiteralExpression_FlowStateAfter(ILiteralExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState FunctionName_FlowStateAfter(IFunctionNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState FieldAccessExpression_FlowStateAfter(IFieldAccessExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState VariableNameExpression_FlowStateAfter(IVariableNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState SelfExpression_FlowStateAfter(ISelfExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState UnknownMemberAccessExpression_FlowStateAfter(IUnknownMemberAccessExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState NamedParameter_FlowStateAfter(INamedParameterNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState ExpressionStatement_FlowStateAfter(IExpressionStatementNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState BlockExpression_FlowStateAfter(IBlockExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState NewObjectExpression_FlowStateAfter(INewObjectExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState UnsafeExpression_FlowStateAfter(IUnsafeExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState AssignmentExpression_FlowStateAfter(IAssignmentExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState BinaryOperatorExpression_FlowStateAfter(IBinaryOperatorExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState UnaryOperatorExpression_FlowStateAfter(IUnaryOperatorExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState IdExpression_FlowStateAfter(IIdExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState ConversionExpression_FlowStateAfter(IConversionExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState ImplicitConversionExpression_FlowStateAfter(IImplicitConversionExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState PatternMatchExpression_FlowStateAfter(IPatternMatchExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState IfExpression_FlowStateAfter(IIfExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState LoopExpression_FlowStateAfter(ILoopExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState WhileExpression_FlowStateAfter(IWhileExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState BreakExpression_FlowStateAfter(IBreakExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState NextExpression_FlowStateAfter(INextExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState ReturnExpression_FlowStateAfter(IReturnExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState FunctionInvocationExpression_FlowStateAfter(IFunctionInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState MethodInvocationExpression_FlowStateAfter(IMethodInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState GetterInvocationExpression_FlowStateAfter(IGetterInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState SetterInvocationExpression_FlowStateAfter(ISetterInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState FunctionReferenceInvocationExpression_FlowStateAfter(IFunctionReferenceInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState InitializerInvocationExpression_FlowStateAfter(IInitializerInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState MoveVariableExpression_FlowStateAfter(IMoveVariableExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState MoveValueExpression_FlowStateAfter(IMoveValueExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState ImplicitTempMoveExpression_FlowStateAfter(IImplicitTempMoveExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState FreezeVariableExpression_FlowStateAfter(IFreezeVariableExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState FreezeValueExpression_FlowStateAfter(IFreezeValueExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState PrepareToReturnExpression_FlowStateAfter(IPrepareToReturnExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState AsyncStartExpression_FlowStateAfter(IAsyncStartExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState AwaitExpression_FlowStateAfter(IAwaitExpressionNode node);
}
