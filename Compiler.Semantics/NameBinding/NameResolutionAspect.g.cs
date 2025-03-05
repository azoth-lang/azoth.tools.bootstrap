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
internal static partial class NameResolutionAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void Validate_FunctionNameExpression(
        INameExpressionSyntax syntax,
        INameNode? context,
        OrdinaryName functionName,
        IEnumerable<ITypeNode> genericArguments,
        IEnumerable<IFunctionInvocableDeclarationNode> referencedDeclarations);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void UnresolvedMemberAccessExpression_Contribute_Diagnostics(IUnresolvedMemberAccessExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedList<IDeclarationNode> UnresolvedOrdinaryNameExpression_ReferencedDeclarations(IUnresolvedOrdinaryNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void UnresolvedOrdinaryNameExpression_Contribute_Diagnostics(IUnresolvedOrdinaryNameExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedList<INamespaceOrTypeDeclarationNode> UnresolvedOrdinaryName_ReferencedDeclarations(IUnresolvedOrdinaryNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void UnresolvedOrdinaryName_Contribute_Diagnostics(IUnresolvedOrdinaryNameNode node, DiagnosticCollectionBuilder diagnostics);
    public static partial IUnresolvedMemberAccessExpressionNode? UnresolvedMemberAccessExpression_Deref_Rewrite_UnresolvedMemberAccessExpression(IUnresolvedMemberAccessExpressionNode node);
    public static partial IExpressionNode? UnresolvedMemberAccessExpression_ExpressionContext_ReplaceWith_Expression(IUnresolvedMemberAccessExpressionNode node);
    public static partial ISetterInvocationExpressionNode? AssignmentExpression_Rewrite_SetterInvocationExpression(IAssignmentExpressionNode node);
    public static partial INameExpressionNode? UnresolvedIdentifierNameExpression_ReplaceWith_NameExpression(IUnresolvedIdentifierNameExpressionNode node);
    public static partial INameExpressionNode? UnresolvedGenericNameExpression_ReplaceWith_NameExpression(IUnresolvedGenericNameExpressionNode node);
    public static partial IUnresolvedNamespaceQualifiedNameExpressionNode? UnresolvedNameExpressionQualifiedNameExpression_ReplaceWith_UnresolvedNamespaceQualifiedNameExpression(IUnresolvedNameExpressionQualifiedNameExpressionNode node);
    public static partial IUnresolvedTypeQualifiedNameExpressionNode? UnresolvedNameExpressionQualifiedNameExpression_ReplaceWith_UnresolvedTypeQualifiedNameExpression(IUnresolvedNameExpressionQualifiedNameExpressionNode node);
    public static partial INameExpressionNode? UnresolvedNamespaceQualifiedNameExpression_ReplaceWith_NameExpression(IUnresolvedNamespaceQualifiedNameExpressionNode node);
    public static partial INameExpressionNode? UnresolvedTypeQualifiedNameExpression_ReplaceWith_NameExpression(IUnresolvedTypeQualifiedNameExpressionNode node);
    public static partial INameNode? UnresolvedIdentifierName_ReplaceWith_Name(IUnresolvedIdentifierNameNode node);
    public static partial INameNode? UnresolvedGenericName_ReplaceWith_Name(IUnresolvedGenericNameNode node);
    public static partial IUnresolvedNamespaceQualifiedNameNode? UnresolvedNameQualifiedName_ReplaceWith_UnresolvedNamespaceQualifiedName(IUnresolvedNameQualifiedNameNode node);
    public static partial IUnresolvedTypeQualifiedNameNode? UnresolvedNameQualifiedName_ReplaceWith_UnresolvedTypeQualifiedName(IUnresolvedNameQualifiedNameNode node);
    public static partial INameNode? UnresolvedNamespaceQualifiedName_ReplaceWith_Name(IUnresolvedNamespaceQualifiedNameNode node);
    public static partial INameNode? UnresolvedTypeQualifiedName_ReplaceWith_Name(IUnresolvedTypeQualifiedNameNode node);
}
