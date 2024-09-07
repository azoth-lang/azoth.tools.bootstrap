using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class BindingAmbiguousNamesAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedList<IDeclarationNode> StandardNameExpression_ReferencedDeclarations(IStandardNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void FunctionGroupName_Contribute_Diagnostics(IFunctionGroupNameNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void UnknownIdentifierNameExpression_Contribute_Diagnostics(IUnknownIdentifierNameExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void UnknownMemberAccessExpression_Contribute_Diagnostics(IUnknownMemberAccessExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    public static partial IExpressionNode? AssignmentExpression_Rewrite_PropertyNameLeftOperand(IAssignmentExpressionNode node);
    public static partial IAmbiguousNameExpressionNode? IdentifierNameExpression_Rewrite(IIdentifierNameExpressionNode node);
    public static partial IAmbiguousNameExpressionNode? MemberAccessExpression_Rewrite_FunctionOrMethodGroupNameContext(IMemberAccessExpressionNode node);
    public static partial IAmbiguousNameExpressionNode? MemberAccessExpression_Rewrite_NamespaceNameContext(IMemberAccessExpressionNode node);
    public static partial IAmbiguousNameExpressionNode? MemberAccessExpression_Rewrite_TypeNameExpressionContext(IMemberAccessExpressionNode node);
    public static partial IAmbiguousNameExpressionNode? MemberAccessExpression_Rewrite_ExpressionContext(IMemberAccessExpressionNode node);
    public static partial IAmbiguousNameExpressionNode? MemberAccessExpression_Rewrite_UnknownNameExpressionContext(IMemberAccessExpressionNode node);
    public static partial IAmbiguousNameExpressionNode? PropertyName_Rewrite(IPropertyNameNode node);
}