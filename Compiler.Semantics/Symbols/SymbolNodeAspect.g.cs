using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class SymbolNodeAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void Validate_ClassSymbolNode(
        UserTypeSymbol symbol);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void Validate_StructSymbolNode(
        UserTypeSymbol symbol);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void Validate_TraitSymbolNode(
        UserTypeSymbol symbol);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void Validate_StandardMethodSymbolNode(
        MethodSymbol symbol);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void Validate_GetterMethodSymbolNode(
        MethodSymbol symbol);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void Validate_SetterMethodSymbolNode(
        MethodSymbol symbol);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IPackageSymbolNode PackageReference_SymbolNode(IPackageReferenceNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ITypeDeclarationNode? StandardTypeName_ReferencedDeclaration(IStandardTypeNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void StandardTypeName_Contribute_Diagnostics(IStandardTypeNameNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial INamespaceSymbolNode PackageFacetSymbol_GlobalNamespace(IPackageFacetSymbolNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ITypeMemberSymbolNode> BuiltInTypeSymbol_Members(IBuiltInTypeSymbolNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedList<IGenericParameterSymbolNode> UserTypeSymbol_GenericParameters(IUserTypeSymbolNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial FixedDictionary<IdentifierName, IPackageDeclarationNode> Package_PackageDeclarations(IPackageNode node);
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IPackageFacetSymbolNode PackageSymbol_MainFacet(IPackageSymbolNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IPackageFacetSymbolNode PackageSymbol_TestingFacet(IPackageSymbolNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedList<INamespaceMemberSymbolNode> NamespaceSymbol_Members(INamespaceSymbolNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<IClassMemberSymbolNode> ClassSymbol_Members(IClassSymbolNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<IStructMemberSymbolNode> StructSymbol_Members(IStructSymbolNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ITraitMemberSymbolNode> TraitSymbol_Members(ITraitSymbolNode node);
}
