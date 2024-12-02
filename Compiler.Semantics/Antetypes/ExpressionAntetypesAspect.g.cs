using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

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
    public static partial IMaybeAntetype? MethodName_Context_ExpectedAntetype(IMethodNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void AwaitExpression_Contribute_Diagnostics(IAwaitExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype ResultStatement_Antetype(IResultStatementNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype BlockExpression_Antetype(IBlockExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype NewObjectExpression_Antetype(INewObjectExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype UnsafeExpression_Antetype(IUnsafeExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype BoolLiteralExpression_Antetype(IBoolLiteralExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype IntegerLiteralExpression_Antetype(IIntegerLiteralExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype NoneLiteralExpression_Antetype(INoneLiteralExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype StringLiteralExpression_Antetype(IStringLiteralExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype AssignmentExpression_Antetype(IAssignmentExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype BinaryOperatorExpression_Antetype(IBinaryOperatorExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IAntetype? BinaryOperatorExpression_NumericOperatorCommonAntetype(IBinaryOperatorExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype UnaryOperatorExpression_Antetype(IUnaryOperatorExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype ConversionExpression_Antetype(IConversionExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype IfExpression_Antetype(IIfExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype LoopExpression_Antetype(ILoopExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype WhileExpression_Antetype(IWhileExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype ForeachExpression_Antetype(IForeachExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype FunctionInvocationExpression_Antetype(IFunctionInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype MethodInvocationExpression_Antetype(IMethodInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype GetterInvocationExpression_Antetype(IGetterInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype SetterInvocationExpression_Antetype(ISetterInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype FunctionReferenceInvocationExpression_Antetype(IFunctionReferenceInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial FunctionPlainType FunctionReferenceInvocationExpression_FunctionPlainType(IFunctionReferenceInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype InitializerInvocationExpression_Antetype(IInitializerInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype FunctionName_Antetype(IFunctionNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype MethodName_Antetype(IMethodNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype FieldAccessExpression_Antetype(IFieldAccessExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype VariableNameExpression_Antetype(IVariableNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype SelfExpression_Antetype(ISelfExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype AsyncStartExpression_Antetype(IAsyncStartExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype AwaitExpression_Antetype(IAwaitExpressionNode node);
    public static partial IExpressionNode? Expression_Rewrite_ImplicitConversion(IExpressionNode node);
}
