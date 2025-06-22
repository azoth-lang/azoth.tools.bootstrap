using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class LexicalScopingAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope BodyOrBlock_Statements_Broadcast_ContainingLexicalScope(IBodyOrBlockNode node, int statementIndex);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial PackageNameScope Package_MainFacet_PackageNameScope(IPackageNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial PackageNameScope Package_TestsFacet_PackageNameScope(IPackageNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope TypeDefinition_AllSupertypeNames_Broadcast_ContainingLexicalScope(ITypeDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope TypeDefinition_Members_Broadcast_ContainingLexicalScope(ITypeDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope TypeDefinition_LexicalScope(ITypeDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope TypeDefinition_SupertypesLexicalScope(ITypeDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope MethodDefinition_LexicalScope(IMethodDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope InitializerDefinition_LexicalScope(IInitializerDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ConditionalLexicalScope AmbiguousExpression_FlowLexicalScope(IAmbiguousExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ConditionalLexicalScope BindingPattern_FlowLexicalScope(IBindingPatternNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ConditionalLexicalScope AssignmentExpression_FlowLexicalScope(IAssignmentExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope BinaryOperatorExpression_RightOperand_Broadcast_ContainingLexicalScope(IBinaryOperatorExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ConditionalLexicalScope BinaryOperatorExpression_FlowLexicalScope(IBinaryOperatorExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ConditionalLexicalScope UnaryOperatorExpression_FlowLexicalScope(IUnaryOperatorExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial NamespaceSearchScope CompilationUnit_LexicalScope(ICompilationUnitNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial NamespaceSearchScope NamespaceBlockDefinition_LexicalScope(INamespaceBlockDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope FunctionDefinition_LexicalScope(IFunctionDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope AssociatedFunctionDefinition_LexicalScope(IAssociatedFunctionDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope VariableDeclarationStatement_LexicalScope(IVariableDeclarationStatementNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ConditionalLexicalScope TypePattern_FlowLexicalScope(ITypePatternNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope ForeachExpression_LexicalScope(IForeachExpressionNode node);
}
