using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Flow;
using AzothType = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class ExpressionTypesAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType? ExpressionBody_ResultStatement_ExpectedType(IExpressionBodyNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState SelfParameter_FlowStateAfter(ISelfParameterNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType ResultStatement_Type(IResultStatementNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void OrdinaryTypedExpression_Contribute_Diagnostics(IOrdinaryTypedExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState UnresolvedMemberAccessExpression_FlowStateAfter(IUnresolvedMemberAccessExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType FieldAccessExpression_LocatorType(IFieldAccessExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState FieldAccessExpression_FlowStateAfter(IFieldAccessExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void FieldAccessExpression_Contribute_Diagnostics(IFieldAccessExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType? MethodAccessExpression_Context_ExpectedType(IMethodAccessExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState LiteralExpression_FlowStateAfter(ILiteralExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void StringLiteralExpression_Contribute_Diagnostics(IStringLiteralExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void AssignmentExpression_Contribute_Diagnostics(IAssignmentExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void BinaryOperatorExpression_Contribute_Diagnostics(IBinaryOperatorExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void ConversionExpression_Contribute_Diagnostics(IConversionExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void IfExpression_Contribute_Diagnostics(IIfExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void ReturnExpression_Contribute_Diagnostics(IReturnExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void FunctionInvocationExpression_Contribute_Diagnostics(IFunctionInvocationExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void MethodInvocationExpression_Contribute_Diagnostics(IMethodInvocationExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState GetterInvocationExpression_FlowStateAfter(IGetterInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void GetterInvocationExpression_Contribute_Diagnostics(IGetterInvocationExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void SetterInvocationExpression_Contribute_Diagnostics(ISetterInvocationExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void FunctionReferenceInvocationExpression_Contribute_Diagnostics(IFunctionReferenceInvocationExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void InitializerInvocationExpression_Contribute_Diagnostics(IInitializerInvocationExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType VariableNameExpression_LocatorType(IVariableNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState VariableNameExpression_FlowStateAfter(IVariableNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState InstanceExpression_FlowStateAfter(IInstanceExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState FunctionNameExpression_FlowStateAfter(IFunctionNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState InitializerNameExpression_FlowStateAfter(IInitializerNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState MissingNameExpression_FlowStateAfter(IMissingNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState UnresolvedNameExpression_FlowStateAfter(IUnresolvedNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState UnresolvedQualifiedNameExpression_FlowStateAfter(IUnresolvedQualifiedNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState NamespaceName_FlowStateAfter(INamespaceNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState TypeName_FlowStateAfter(ITypeNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType MoveExpression_Type(IMoveExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void MoveVariableExpression_Contribute_Diagnostics(IMoveVariableExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void MoveValueExpression_Contribute_Diagnostics(IMoveValueExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType FreezeExpression_Type(IFreezeExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void FreezeVariableExpression_Contribute_Diagnostics(IFreezeVariableExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void FreezeValueExpression_Contribute_Diagnostics(IFreezeValueExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState NamedParameter_FlowStateAfter(INamedParameterNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState ExpressionStatement_FlowStateAfter(IExpressionStatementNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState TypePattern_FlowStateAfter(ITypePatternNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType BlockExpression_Type(IBlockExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState BlockExpression_FlowStateAfter(IBlockExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType UnsafeExpression_Type(IUnsafeExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState UnsafeExpression_FlowStateAfter(IUnsafeExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType FieldAccessExpression_Type(IFieldAccessExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType MethodAccessExpression_Type(IMethodAccessExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial CapabilityType BoolLiteralExpression_Type(IBoolLiteralExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial CapabilityType IntegerLiteralExpression_Type(IIntegerLiteralExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial OptionalType NoneLiteralExpression_Type(INoneLiteralExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType StringLiteralExpression_Type(IStringLiteralExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType AssignmentExpression_Type(IAssignmentExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState AssignmentExpression_FlowStateAfter(IAssignmentExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType RefAssignmentExpression_Type(IRefAssignmentExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState RefAssignmentExpression_FlowStateAfter(IRefAssignmentExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType BinaryOperatorExpression_Type(IBinaryOperatorExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState BinaryOperatorExpression_FlowStateAfter(IBinaryOperatorExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType UnaryOperatorExpression_Type(IUnaryOperatorExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState UnaryOperatorExpression_FlowStateAfter(IUnaryOperatorExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType ConversionExpression_Type(IConversionExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState ConversionExpression_FlowStateAfter(IConversionExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState ImplicitConversionExpression_FlowStateAfter(IImplicitConversionExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType ImplicitConversionExpression_Type(IImplicitConversionExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType OptionalConversionExpression_Type(IOptionalConversionExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState OptionalConversionExpression_FlowStateAfter(IOptionalConversionExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState PatternMatchExpression_FlowStateAfter(IPatternMatchExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType IfExpression_Type(IIfExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState IfExpression_FlowStateAfterCondition(IIfExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState IfExpression_FlowStateAfter(IIfExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType LoopExpression_Type(ILoopExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState LoopExpression_FlowStateAfter(ILoopExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType WhileExpression_Type(IWhileExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState WhileExpression_FlowStateAfter(IWhileExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState WhileExpression_FlowStateAfterCondition(IWhileExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState BreakExpression_FlowStateAfter(IBreakExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState NextExpression_FlowStateAfter(INextExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState ReturnExpression_FlowStateAfter(IReturnExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState UnresolvedInvocationExpression_FlowStateAfter(IUnresolvedInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType FunctionInvocationExpression_Type(IFunctionInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState FunctionInvocationExpression_FlowStateAfter(IFunctionInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ContextualizedCall? FunctionInvocationExpression_ContextualizedCall(IFunctionInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType MethodInvocationExpression_Type(IMethodInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState MethodInvocationExpression_FlowStateAfter(IMethodInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ContextualizedCall? MethodInvocationExpression_ContextualizedCall(IMethodInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType GetterInvocationExpression_Type(IGetterInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ContextualizedCall? GetterInvocationExpression_ContextualizedCall(IGetterInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType SetterInvocationExpression_Type(ISetterInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState SetterInvocationExpression_FlowStateAfter(ISetterInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ContextualizedCall? SetterInvocationExpression_ContextualizedCall(ISetterInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType FunctionReferenceInvocationExpression_Type(IFunctionReferenceInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState FunctionReferenceInvocationExpression_FlowStateAfter(IFunctionReferenceInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial FunctionType FunctionReferenceInvocationExpression_FunctionType(IFunctionReferenceInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType InitializerInvocationExpression_Type(IInitializerInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState InitializerInvocationExpression_FlowStateAfter(IInitializerInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ContextualizedCall? InitializerInvocationExpression_ContextualizedCall(IInitializerInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState NonInvocableInvocationExpression_FlowStateAfter(INonInvocableInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType VariableNameExpression_Type(IVariableNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType SelfExpression_Type(ISelfExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType BaseExpression_Type(IBaseExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType FunctionNameExpression_Type(IFunctionNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType InitializerNameExpression_Type(IInitializerNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState MoveVariableExpression_FlowStateAfter(IMoveVariableExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState MoveValueExpression_FlowStateAfter(IMoveValueExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType ImplicitTempMoveExpression_Type(IImplicitTempMoveExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState ImplicitTempMoveExpression_FlowStateAfter(IImplicitTempMoveExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState FreezeVariableExpression_FlowStateAfter(IFreezeVariableExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState FreezeValueExpression_FlowStateAfter(IFreezeValueExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState PrepareToReturnExpression_FlowStateAfter(IPrepareToReturnExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType AsyncStartExpression_Type(IAsyncStartExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState AsyncStartExpression_FlowStateAfter(IAsyncStartExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType AwaitExpression_Type(IAwaitExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState AwaitExpression_FlowStateAfter(IAwaitExpressionNode node);
    public static partial IExpressionNode? OrdinaryTypedExpression_ImplicitMove_Insert(IOrdinaryTypedExpressionNode node);
    public static partial IFreezeExpressionNode? OrdinaryTypedExpression_Insert_FreezeExpression(IOrdinaryTypedExpressionNode node);
    public static partial IPrepareToReturnExpressionNode? OrdinaryTypedExpression_Insert_PrepareToReturnExpression(IOrdinaryTypedExpressionNode node);
}
