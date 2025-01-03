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
    public static partial void NewObjectExpression_Contribute_Diagnostics(INewObjectExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePlainType? UnresolvedInvocationExpression_Expression_ExpectedPlainType(IUnresolvedInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void UnresolvedInvocationExpression_Contribute_Diagnostics(IUnresolvedInvocationExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void MethodInvocationExpression_Contribute_Diagnostics(IMethodInvocationExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void FunctionGroupName_Contribute_Diagnostics(IFunctionGroupNameNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void FunctionName_Contribute_Diagnostics(IFunctionNameNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void MethodGroupName_Contribute_Diagnostics(IMethodGroupNameNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void InitializerGroupName_Contribute_Diagnostics(IInitializerGroupNameNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<IConstructorDeclarationNode> NewObjectExpression_CompatibleConstructors(INewObjectExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IConstructorDeclarationNode? NewObjectExpression_ReferencedConstructor(INewObjectExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ICallCandidate<IPropertyAccessorDeclarationNode>> GetterInvocationExpression_CallCandidates(IGetterInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ICallCandidate<IGetterMethodDeclarationNode>> GetterInvocationExpression_CompatibleCallCandidates(IGetterInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ICallCandidate<IPropertyAccessorDeclarationNode>> SetterInvocationExpression_CallCandidates(ISetterInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ICallCandidate<ISetterMethodDeclarationNode>> SetterInvocationExpression_CompatibleCallCandidates(ISetterInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ICallCandidate<IFunctionInvocableDeclarationNode>> FunctionGroupName_CallCandidates(IFunctionGroupNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ICallCandidate<IFunctionInvocableDeclarationNode>> FunctionGroupName_CompatibleCallCandidates(IFunctionGroupNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ICallCandidate<IFunctionInvocableDeclarationNode>? FunctionGroupName_SelectedCallCandidate(IFunctionGroupNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ICallCandidate<IOrdinaryMethodDeclarationNode>> MethodGroupName_CallCandidates(IMethodGroupNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ICallCandidate<IOrdinaryMethodDeclarationNode>> MethodGroupName_CompatibleCallCandidates(IMethodGroupNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ICallCandidate<IOrdinaryMethodDeclarationNode>? MethodGroupName_SelectedCallCandidate(IMethodGroupNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ICallCandidate<IInitializerDeclarationNode>> InitializerGroupName_CallCandidates(IInitializerGroupNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ICallCandidate<IInitializerDeclarationNode>> InitializerGroupName_CompatibleCallCandidates(IInitializerGroupNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ICallCandidate<IInitializerDeclarationNode>? InitializerGroupName_SelectedCallCandidate(IInitializerGroupNameNode node);
    public static partial IExpressionNode? UnresolvedInvocationExpression_Rewrite_FunctionNameExpression(IUnresolvedInvocationExpressionNode node);
    public static partial IExpressionNode? UnresolvedInvocationExpression_Rewrite_MethodNameExpression(IUnresolvedInvocationExpressionNode node);
    public static partial IExpressionNode? UnresolvedInvocationExpression_Rewrite_TypeNameExpression(IUnresolvedInvocationExpressionNode node);
    public static partial IExpressionNode? UnresolvedInvocationExpression_Rewrite_InitializerNameExpression(IUnresolvedInvocationExpressionNode node);
    public static partial IExpressionNode? UnresolvedInvocationExpression_Rewrite_FunctionReferenceExpression(IUnresolvedInvocationExpressionNode node);
    public static partial IExpressionNode? UnresolvedInvocationExpression_Rewrite_Unbound(IUnresolvedInvocationExpressionNode node);
}
