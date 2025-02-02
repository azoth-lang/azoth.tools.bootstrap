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
    public static partial ControlFlowSet Expression_ControlFlowNext(IExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet BlockExpression_ControlFlowNext(IBlockExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet UnsafeExpression_ControlFlowNext(IUnsafeExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet UnresolvedMemberAccessExpression_ControlFlowNext(IUnresolvedMemberAccessExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet FieldAccessExpression_ControlFlowNext(IFieldAccessExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet MethodAccessExpression_ControlFlowNext(IMethodAccessExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet AssignmentExpression_ControlFlowNext(IAssignmentExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet BinaryOperatorExpression_ControlFlowNext(IBinaryOperatorExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet UnaryOperatorExpression_ControlFlowNext(IUnaryOperatorExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet ConversionExpression_ControlFlowNext(IConversionExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet ImplicitConversionExpression_ControlFlowNext(IImplicitConversionExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet PatternMatchExpression_ControlFlowNext(IPatternMatchExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet RefExpression_ControlFlowNext(IRefExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet ImplicitDerefExpression_ControlFlowNext(IImplicitDerefExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet IfExpression_ControlFlowNext(IIfExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet LoopExpression_ControlFlowNext(ILoopExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet WhileExpression_ControlFlowNext(IWhileExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet ForeachExpression_ControlFlowNext(IForeachExpressionNode node);
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
    public static partial ControlFlowSet RecoveryExpression_ControlFlowNext(IRecoveryExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet ImplicitTempMoveExpression_ControlFlowNext(IImplicitTempMoveExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet PrepareToReturnExpression_ControlFlowNext(IPrepareToReturnExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet AwaitExpression_ControlFlowNext(IAwaitExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet Entry_ControlFlowNext(IEntryNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet ResultStatement_ControlFlowNext(IResultStatementNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet VariableDeclarationStatement_ControlFlowNext(IVariableDeclarationStatementNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet ExpressionStatement_ControlFlowNext(IExpressionStatementNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet BindingContextPattern_ControlFlowNext(IBindingContextPatternNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet BindingPattern_ControlFlowNext(IBindingPatternNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet OptionalPattern_ControlFlowNext(IOptionalPatternNode node);
}
