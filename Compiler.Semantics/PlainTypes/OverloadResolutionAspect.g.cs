using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.PlainTypes;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class OverloadResolutionAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void MethodAccessExpression_Contribute_Diagnostics(IMethodAccessExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePlainType? UnresolvedInvocationExpression_Expression_ExpectedPlainType(IUnresolvedInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void UnresolvedInvocationExpression_Contribute_Diagnostics(IUnresolvedInvocationExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePlainType? FunctionInvocationExpression_Function_ExpectedPlainType(IFunctionInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePlainType? MethodInvocationExpression_Method_ExpectedPlainType(IMethodInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePlainType? InitializerInvocationExpression_Initializer_ExpectedPlainType(IInitializerInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void FunctionNameExpression_Contribute_Diagnostics(IFunctionNameExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void InitializerNameExpression_Contribute_Diagnostics(IInitializerNameExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ICallCandidate<IOrdinaryMethodDeclarationNode>> MethodAccessExpression_CallCandidates(IMethodAccessExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ICallCandidate<IOrdinaryMethodDeclarationNode>> MethodAccessExpression_CompatibleCallCandidates(IMethodAccessExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ICallCandidate<IOrdinaryMethodDeclarationNode>? MethodAccessExpression_SelectedCallCandidate(IMethodAccessExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ICallCandidate<IPropertyAccessorDeclarationNode>> GetterInvocationExpression_CallCandidates(IGetterInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ICallCandidate<IGetterMethodDeclarationNode>> GetterInvocationExpression_CompatibleCallCandidates(IGetterInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ICallCandidate<IPropertyAccessorDeclarationNode>> SetterInvocationExpression_CallCandidates(ISetterInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ICallCandidate<ISetterMethodDeclarationNode>> SetterInvocationExpression_CompatibleCallCandidates(ISetterInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ICallCandidate<IFunctionInvocableDeclarationNode>> FunctionNameExpression_CallCandidates(IFunctionNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ICallCandidate<IFunctionInvocableDeclarationNode>> FunctionNameExpression_CompatibleCallCandidates(IFunctionNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ICallCandidate<IFunctionInvocableDeclarationNode>? FunctionNameExpression_SelectedCallCandidate(IFunctionNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ICallCandidate<IInitializerDeclarationNode>> InitializerNameExpression_CallCandidates(IInitializerNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ICallCandidate<IInitializerDeclarationNode>> InitializerNameExpression_CompatibleCallCandidates(IInitializerNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ICallCandidate<IInitializerDeclarationNode>? InitializerNameExpression_SelectedCallCandidate(IInitializerNameExpressionNode node);
    public static partial IFunctionInvocationExpressionNode? UnresolvedInvocationExpression_ReplaceWith_FunctionInvocationExpression(IUnresolvedInvocationExpressionNode node);
    public static partial IMethodInvocationExpressionNode? UnresolvedInvocationExpression_ReplaceWith_MethodInvocationExpression(IUnresolvedInvocationExpressionNode node);
    public static partial IExpressionNode? UnresolvedInvocationExpression_TypeName_Rewrite(IUnresolvedInvocationExpressionNode node);
    public static partial IInitializerInvocationExpressionNode? UnresolvedInvocationExpression_ReplaceWith_InitializerInvocationExpression(IUnresolvedInvocationExpressionNode node);
    public static partial IFunctionReferenceInvocationExpressionNode? UnresolvedInvocationExpression_ReplaceWith_FunctionReferenceInvocationExpression(IUnresolvedInvocationExpressionNode node);
    public static partial INonInvocableInvocationExpressionNode? UnresolvedInvocationExpression_ReplaceWith_NonInvocableInvocationExpression(IUnresolvedInvocationExpressionNode node);
}
