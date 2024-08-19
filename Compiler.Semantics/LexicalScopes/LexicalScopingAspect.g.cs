using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class LexicalScopingAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope BodyOrBlock_Statements_ContainingLexicalScope(IBodyOrBlockNode node, int statementIndex);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial PackageNameScope Package_MainFacet_PackageNameScope(IPackageNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial PackageNameScope Package_TestingFacet_PackageNameScope(IPackageNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope TypeDefinition_LexicalScope(ITypeDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope TypeDefinition_SupertypesLexicalScope(ITypeDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope MethodDefinition_LexicalScope(IMethodDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope ConstructorDefinition_LexicalScope(IConstructorDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope InitializerDefinition_LexicalScope(IInitializerDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope CompilationUnit_LexicalScope(ICompilationUnitNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope NamespaceBlockDefinition_LexicalScope(INamespaceBlockDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope FunctionDefinition_LexicalScope(IFunctionDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope FieldDefinition_LexicalScope(IFieldDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope AssociatedFunctionDefinition_LexicalScope(IAssociatedFunctionDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope VariableDeclarationStatement_LexicalScope(IVariableDeclarationStatementNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial LexicalScope ForeachExpression_LexicalScope(IForeachExpressionNode node);
}
