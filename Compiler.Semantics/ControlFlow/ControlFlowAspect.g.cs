using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class ControlFlowAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IEntryNode ExecutableDefinition_Entry(IExecutableDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IExitNode ExecutableDefinition_Exit(IExecutableDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet InvocableDefinition_Entry_ControlFlowFollowing(IInvocableDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(IControlFlowNode node, IControlFlowNode target, Dictionary<IControlFlowNode, ControlFlowKind> controlFlowPrevious);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet UnresolvedMemberAccessExpression_ControlFlowNext(IUnresolvedMemberAccessExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet LiteralExpression_ControlFlowNext(ILiteralExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet NameExpression_ControlFlowNext(INameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet UnresolvedQualifiedNameExpression_ControlFlowNext(IUnresolvedQualifiedNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet RecoveryExpression_ControlFlowNext(IRecoveryExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet Entry_ControlFlowNext(IEntryNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet ResultStatement_ControlFlowNext(IResultStatementNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet VariableDeclarationStatement_ControlFlowNext(IVariableDeclarationStatementNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet ExpressionStatement_ControlFlowNext(IExpressionStatementNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet TypePattern_ControlFlowNext(ITypePatternNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet BindingContextPattern_ControlFlowNext(IBindingContextPatternNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet BindingPattern_ControlFlowNext(IBindingPatternNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet OptionalPattern_ControlFlowNext(IOptionalPatternNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet BlockExpression_ControlFlowNext(IBlockExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet UnsafeExpression_ControlFlowNext(IUnsafeExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet FieldAccessExpression_ControlFlowNext(IFieldAccessExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet MethodAccessExpression_ControlFlowNext(IMethodAccessExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet AssignmentExpression_ControlFlowNext(IAssignmentExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet RefAssignmentExpression_ControlFlowNext(IRefAssignmentExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet BinaryOperatorExpression_ControlFlowNext(IBinaryOperatorExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet UnaryOperatorExpression_ControlFlowNext(IUnaryOperatorExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet ConversionExpression_ControlFlowNext(IConversionExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet ImplicitConversionExpression_ControlFlowNext(IImplicitConversionExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet OptionalConversionExpression_ControlFlowNext(IOptionalConversionExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet PatternMatchExpression_ControlFlowNext(IPatternMatchExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet IfExpression_ControlFlowNext(IIfExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet LoopExpression_ControlFlowNext(ILoopExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet WhileExpression_ControlFlowNext(IWhileExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet ForeachExpression_ControlFlowNext(IForeachExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet BreakExpression_ControlFlowNext(IBreakExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet NextExpression_ControlFlowNext(INextExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet ReturnExpression_ControlFlowNext(IReturnExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet UnresolvedInvocationExpression_ControlFlowNext(IUnresolvedInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet FunctionInvocationExpression_ControlFlowNext(IFunctionInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet MethodInvocationExpression_ControlFlowNext(IMethodInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet GetterInvocationExpression_ControlFlowNext(IGetterInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet SetterInvocationExpression_ControlFlowNext(ISetterInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet FunctionReferenceInvocationExpression_ControlFlowNext(IFunctionReferenceInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet InitializerInvocationExpression_ControlFlowNext(IInitializerInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet NonInvocableInvocationExpression_ControlFlowNext(INonInvocableInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet ImplicitTempMoveExpression_ControlFlowNext(IImplicitTempMoveExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet PrepareToReturnExpression_ControlFlowNext(IPrepareToReturnExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet AsyncBlockExpression_ControlFlowNext(IAsyncBlockExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet AsyncStartExpression_ControlFlowNext(IAsyncStartExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet AwaitExpression_ControlFlowNext(IAwaitExpressionNode node);
}
