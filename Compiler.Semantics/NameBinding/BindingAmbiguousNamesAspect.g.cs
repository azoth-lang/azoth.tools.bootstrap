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
internal static partial class BindingAmbiguousNamesAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void Validate_FunctionGroupNameNode(
        INameExpressionSyntax syntax,
        INameExpressionNode? context,
        OrdinaryName functionName,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IFunctionInvocableDeclarationNode> referencedDeclarations);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void UnresolvedMemberAccessExpression_Contribute_Diagnostics(IUnresolvedMemberAccessExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedList<IDeclarationNode> OrdinaryNameExpression_ReferencedDeclarations(IOrdinaryNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void FunctionGroupName_Contribute_Diagnostics(IFunctionGroupNameNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void MethodGroupName_Contribute_Diagnostics(IMethodGroupNameNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void UnknownIdentifierNameExpression_Contribute_Diagnostics(IUnknownIdentifierNameExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    public static partial IExpressionNode? UnresolvedMemberAccessExpression_NamespaceNameContext_ReplaceWith_Expression(IUnresolvedMemberAccessExpressionNode node);
    public static partial IExpressionNode? UnresolvedMemberAccessExpression_TypeNameExpressionContext_ReplaceWith_Expression(IUnresolvedMemberAccessExpressionNode node);
    public static partial INameExpressionNode? UnresolvedMemberAccessExpression_ExpressionContext_ReplaceWith_NameExpression(IUnresolvedMemberAccessExpressionNode node);
    public static partial IExpressionNode? AssignmentExpression_PropertyNameLeftOperand_Rewrite(IAssignmentExpressionNode node);
    public static partial INameExpressionNode? IdentifierNameExpression_ReplaceWith_NameExpression(IIdentifierNameExpressionNode node);
    public static partial INameExpressionNode? GenericNameExpression_ReplaceWith_NameExpression(IGenericNameExpressionNode node);
    public static partial IFunctionNameNode? FunctionGroupName_ReplaceWith_FunctionName(IFunctionGroupNameNode node);
    public static partial IMethodNameNode? MethodGroupName_ReplaceWith_MethodName(IMethodGroupNameNode node);
    public static partial IInitializerNameNode? InitializerGroupName_ReplaceWith_InitializerName(IInitializerGroupNameNode node);
}
