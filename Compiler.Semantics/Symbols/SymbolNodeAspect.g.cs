using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

// ReSharper disable PartialTypeWithSinglePart
#nullable enable

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class SymbolNodeAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial INamespaceDefinitionNode CompilationUnit_Children_ContainingDeclaration(ICompilationUnitNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ITypeDeclarationNode? StandardTypeName_ReferencedDeclaration(IStandardTypeNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial FixedDictionary<IdentifierName, IPackageDeclarationNode> Package_PackageDeclarations(IPackageNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IPackageSymbolNode PackageReference_SymbolNode(IPackageReferenceNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial INamespaceDefinitionNode PackageFacet_GlobalNamespace(IPackageFacetNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial INamespaceDefinitionNode CompilationUnit_ImplicitNamespace(ICompilationUnitNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial INamespaceDefinitionNode NamespaceBlockDefinition_ContainingNamespace(INamespaceBlockDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial INamespaceDefinitionNode NamespaceBlockDefinition_Definition(INamespaceBlockDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFieldDefinitionNode? FieldParameter_ReferencedField(IFieldParameterNode node);
}
