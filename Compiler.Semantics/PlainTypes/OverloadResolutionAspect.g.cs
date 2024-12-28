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
    public static partial IFixedSet<IConstructorDeclarationNode> NewObjectExpression_CompatibleConstructors(INewObjectExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IConstructorDeclarationNode? NewObjectExpression_ReferencedConstructor(INewObjectExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<IInitializerDeclarationNode> InitializerInvocationExpression_CompatibleDeclarations(IInitializerInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IInitializerDeclarationNode? InitializerInvocationExpression_ReferencedDeclaration(IInitializerInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<CallCandidate<IFunctionInvocableDeclarationNode>> FunctionGroupName_CallCandidates(IFunctionGroupNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<CallCandidate<IFunctionInvocableDeclarationNode>> FunctionGroupName_CompatibleCallCandidates(IFunctionGroupNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial CallCandidate<IFunctionInvocableDeclarationNode>? FunctionGroupName_SelectedCallCandidate(IFunctionGroupNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFunctionInvocableDeclarationNode? FunctionGroupName_ReferencedDeclaration(IFunctionGroupNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<CallCandidate<IOrdinaryMethodDeclarationNode>> MethodGroupName_CallCandidates(IMethodGroupNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<CallCandidate<IOrdinaryMethodDeclarationNode>> MethodGroupName_CompatibleCallCandidates(IMethodGroupNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial CallCandidate<IOrdinaryMethodDeclarationNode>? MethodGroupName_SelectedCallCandidate(IMethodGroupNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IOrdinaryMethodDeclarationNode? MethodGroupName_ReferencedDeclaration(IMethodGroupNameNode node);
    public static partial IExpressionNode? UnresolvedInvocationExpression_Rewrite_FunctionNameExpression(IUnresolvedInvocationExpressionNode node);
    public static partial IExpressionNode? UnresolvedInvocationExpression_Rewrite_MethodNameExpression(IUnresolvedInvocationExpressionNode node);
    public static partial IExpressionNode? UnresolvedInvocationExpression_Rewrite_TypeNameExpression(IUnresolvedInvocationExpressionNode node);
    public static partial IExpressionNode? UnresolvedInvocationExpression_Rewrite_InitializerGroupNameExpression(IUnresolvedInvocationExpressionNode node);
    public static partial IExpressionNode? UnresolvedInvocationExpression_Rewrite_FunctionReferenceExpression(IUnresolvedInvocationExpressionNode node);
    public static partial IExpressionNode? UnresolvedInvocationExpression_Rewrite_Unbound(IUnresolvedInvocationExpressionNode node);
}
