using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class BindingUnresolvedNamesAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void Validate_FunctionGroupNameNode(
        INameExpressionSyntax syntax,
        IResolvedNameNode? context,
        OrdinaryName functionName,
        IEnumerable<ITypeNode> genericArguments,
        IEnumerable<IFunctionInvocableDeclarationNode> referencedDeclarations);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void UnresolvedMemberAccessExpression_Contribute_Diagnostics(IUnresolvedMemberAccessExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void FunctionGroupName_Contribute_Diagnostics(IFunctionGroupNameNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void MethodGroupName_Contribute_Diagnostics(IMethodGroupNameNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedList<IDeclarationNode> UnresolvedOrdinaryNameExpression_ReferencedDeclarations(IUnresolvedOrdinaryNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void UnresolvedOrdinaryNameExpression_Contribute_Diagnostics(IUnresolvedOrdinaryNameExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    public static partial IExpressionNode? UnresolvedMemberAccessExpression_ExpressionContext_ReplaceWith_Expression(IUnresolvedMemberAccessExpressionNode node);
    public static partial IExpressionNode? AssignmentExpression_PropertyNameLeftOperand_Rewrite(IAssignmentExpressionNode node);
    public static partial IFunctionNameExpressionNode? FunctionGroupName_ReplaceWith_FunctionNameExpression(IFunctionGroupNameNode node);
    public static partial IMethodNameNode? MethodGroupName_ReplaceWith_MethodName(IMethodGroupNameNode node);
    public static partial IInitializerNameExpressionNode? InitializerGroupName_ReplaceWith_InitializerNameExpression(IInitializerGroupNameNode node);
    public static partial INameExpressionNode? UnresolvedIdentifierNameExpression_ReplaceWith_NameExpression(IUnresolvedIdentifierNameExpressionNode node);
    public static partial INameExpressionNode? UnresolvedGenericNameExpression_ReplaceWith_NameExpression(IUnresolvedGenericNameExpressionNode node);
    public static partial IUnresolvedNamespaceQualifiedNameExpressionNode? UnresolvedQualifiedNameExpression_ReplaceWith_UnresolvedNamespaceQualifiedNameExpression(IUnresolvedQualifiedNameExpressionNode node);
    public static partial IUnresolvedTypeQualifiedNameExpressionNode? UnresolvedQualifiedNameExpression_ReplaceWith_UnresolvedTypeQualifiedNameExpression(IUnresolvedQualifiedNameExpressionNode node);
    public static partial INameExpressionNode? UnresolvedNamespaceQualifiedNameExpression_ReplaceWith_NameExpression(IUnresolvedNamespaceQualifiedNameExpressionNode node);
    public static partial INameExpressionNode? UnresolvedTypeQualifiedNameExpression_ReplaceWith_NameExpression(IUnresolvedTypeQualifiedNameExpressionNode node);
}
