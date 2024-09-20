using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class ExpressionAntetypesAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void OptionalPattern_Contribute_Diagnostics(IOptionalPatternNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void UnaryOperatorExpression_Contribute_Diagnostics(IUnaryOperatorExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype? MethodName_Context_ExpectedAntetype(IMethodNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void AwaitExpression_Contribute_Diagnostics(IAwaitExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype ResultStatement_Antetype(IResultStatementNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype BlockExpression_Antetype(IBlockExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype NewObjectExpression_Antetype(INewObjectExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype UnsafeExpression_Antetype(IUnsafeExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype BoolLiteralExpression_Antetype(IBoolLiteralExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype IntegerLiteralExpression_Antetype(IIntegerLiteralExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype NoneLiteralExpression_Antetype(INoneLiteralExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype StringLiteralExpression_Antetype(IStringLiteralExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype AssignmentExpression_Antetype(IAssignmentExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype BinaryOperatorExpression_Antetype(IBinaryOperatorExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IAntetype? BinaryOperatorExpression_NumericOperatorCommonAntetype(IBinaryOperatorExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype UnaryOperatorExpression_Antetype(IUnaryOperatorExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype ConversionExpression_Antetype(IConversionExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype IfExpression_Antetype(IIfExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype LoopExpression_Antetype(ILoopExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype WhileExpression_Antetype(IWhileExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype ForeachExpression_Antetype(IForeachExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype FunctionInvocationExpression_Antetype(IFunctionInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype MethodInvocationExpression_Antetype(IMethodInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype GetterInvocationExpression_Antetype(IGetterInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype SetterInvocationExpression_Antetype(ISetterInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype FunctionReferenceInvocationExpression_Antetype(IFunctionReferenceInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial FunctionAntetype FunctionReferenceInvocationExpression_FunctionAntetype(IFunctionReferenceInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype InitializerInvocationExpression_Antetype(IInitializerInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype FunctionName_Antetype(IFunctionNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype MethodName_Antetype(IMethodNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype FieldAccessExpression_Antetype(IFieldAccessExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype VariableNameExpression_Antetype(IVariableNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype SelfExpression_Antetype(ISelfExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype AsyncStartExpression_Antetype(IAsyncStartExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionAntetype AwaitExpression_Antetype(IAwaitExpressionNode node);
    public static partial IExpressionNode? Expression_Rewrite_ImplicitConversion(IExpressionNode node);
}
