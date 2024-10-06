using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
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
    public static partial IMaybeAntetype? UnknownInvocationExpression_Expression_ExpectedAntetype(IUnknownInvocationExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void UnknownInvocationExpression_Contribute_Diagnostics(IUnknownInvocationExpressionNode node, DiagnosticCollectionBuilder diagnostics);
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
    public static partial IFixedSet<IFunctionInvocableDeclarationNode> FunctionGroupName_CompatibleDeclarations(IFunctionGroupNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFunctionInvocableDeclarationNode? FunctionGroupName_ReferencedDeclaration(IFunctionGroupNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<IStandardMethodDeclarationNode> MethodGroupName_CompatibleDeclarations(IMethodGroupNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IStandardMethodDeclarationNode? MethodGroupName_ReferencedDeclaration(IMethodGroupNameNode node);
    public static partial IExpressionNode? UnknownInvocationExpression_Rewrite_FunctionNameExpression(IUnknownInvocationExpressionNode node);
    public static partial IExpressionNode? UnknownInvocationExpression_Rewrite_MethodNameExpression(IUnknownInvocationExpressionNode node);
    public static partial IExpressionNode? UnknownInvocationExpression_Rewrite_TypeNameExpression(IUnknownInvocationExpressionNode node);
    public static partial IExpressionNode? UnknownInvocationExpression_Rewrite_InitializerGroupNameExpression(IUnknownInvocationExpressionNode node);
    public static partial IExpressionNode? UnknownInvocationExpression_Rewrite_FunctionReferenceExpression(IUnknownInvocationExpressionNode node);
}
