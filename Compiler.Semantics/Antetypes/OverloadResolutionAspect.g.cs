using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class OverloadResolutionAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void NewObjectExpression_Contribute_Diagnostics(INewObjectExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void FunctionInvocationExpression_Contribute_Diagnostics(IFunctionInvocationExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void MethodInvocationExpression_Contribute_Diagnostics(IMethodInvocationExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<IConstructorDeclarationNode> NewObjectExpression_CompatibleConstructors(INewObjectExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IConstructorDeclarationNode? NewObjectExpression_ReferencedConstructor(INewObjectExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<IFunctionInvocableDeclarationNode> FunctionInvocationExpression_CompatibleDeclarations(IFunctionInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFunctionInvocableDeclarationNode? FunctionInvocationExpression_ReferencedDeclaration(IFunctionInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<IStandardMethodDeclarationNode> MethodInvocationExpression_CompatibleDeclarations(IMethodInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IStandardMethodDeclarationNode? MethodInvocationExpression_ReferencedDeclaration(IMethodInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<IInitializerDeclarationNode> InitializerInvocationExpression_CompatibleDeclarations(IInitializerInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IInitializerDeclarationNode? InitializerInvocationExpression_ReferencedDeclaration(IInitializerInvocationExpressionNode node);
    public static partial IAmbiguousExpressionNode? UnresolvedInvocationExpression_Rewrite_FunctionGroupNameExpression(IUnresolvedInvocationExpressionNode node);
    public static partial IAmbiguousExpressionNode? UnresolvedInvocationExpression_Rewrite_MethodGroupNameExpression(IUnresolvedInvocationExpressionNode node);
    public static partial IAmbiguousExpressionNode? UnresolvedInvocationExpression_Rewrite_TypeNameExpression(IUnresolvedInvocationExpressionNode node);
    public static partial IAmbiguousExpressionNode? UnresolvedInvocationExpression_Rewrite_InitializerGroupNameExpression(IUnresolvedInvocationExpressionNode node);
    public static partial IAmbiguousExpressionNode? UnresolvedInvocationExpression_Rewrite_FunctionReferenceExpression(IUnresolvedInvocationExpressionNode node);
    public static partial IAmbiguousExpressionNode? UnresolvedInvocationExpression_Rewrite_ToUnknown(IUnresolvedInvocationExpressionNode node);
}
