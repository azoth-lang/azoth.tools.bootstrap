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
    public static partial INameExpressionNode? UnresolvedMemberAccessExpression_ExpressionContext_ReplaceWith_NameExpression(IUnresolvedMemberAccessExpressionNode node);
    public static partial IExpressionNode? AssignmentExpression_PropertyNameLeftOperand_Rewrite(IAssignmentExpressionNode node);
    public static partial IFunctionNameNode? FunctionGroupName_ReplaceWith_FunctionName(IFunctionGroupNameNode node);
    public static partial IMethodNameNode? MethodGroupName_ReplaceWith_MethodName(IMethodGroupNameNode node);
    public static partial IInitializerNameNode? InitializerGroupName_ReplaceWith_InitializerName(IInitializerGroupNameNode node);
    public static partial INameExpressionNode? UnresolvedIdentifierName_ReplaceWith_NameExpression(IUnresolvedIdentifierNameNode node);
    public static partial INameExpressionNode? UnresolvedGenericName_ReplaceWith_NameExpression(IUnresolvedGenericNameNode node);
    public static partial IUnresolvedNamespaceQualifiedNameNode? UnresolvedQualifiedName_ReplaceWith_UnresolvedNamespaceQualifiedName(IUnresolvedQualifiedNameNode node);
    public static partial IUnresolvedTypeQualifiedNameNode? UnresolvedQualifiedName_ReplaceWith_UnresolvedTypeQualifiedName(IUnresolvedQualifiedNameNode node);
    public static partial INameExpressionNode? UnresolvedNamespaceQualifiedName_ReplaceWith_NameExpression(IUnresolvedNamespaceQualifiedNameNode node);
    public static partial INameExpressionNode? UnresolvedTypeQualifiedName_ReplaceWith_NameExpression(IUnresolvedTypeQualifiedNameNode node);
}
