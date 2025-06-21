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
    public static partial void Validate_ClassSymbol(OrdinaryTypeSymbol symbol);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void Validate_StructSymbol(OrdinaryTypeSymbol symbol);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void Validate_TraitSymbol(OrdinaryTypeSymbol symbol);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void Validate_OrdinaryMethodSymbol(MethodSymbol symbol);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void Validate_GetterMethodSymbol(MethodSymbol symbol);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void Validate_SetterMethodSymbol(MethodSymbol symbol);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IPackageFacetSymbolNode PackageFacetReference_SymbolNode(IPackageFacetReferenceNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial INamespaceSymbolNode PackageFacetSymbol_GlobalNamespace(IPackageFacetSymbolNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ISelfSymbolNode NonVariableTypeSymbol_ImplicitSelf(INonVariableTypeSymbolNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ITypeMemberSymbolNode> BuiltInTypeSymbol_Members(IBuiltInTypeSymbolNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedList<IGenericParameterSymbolNode> OrdinaryTypeSymbol_GenericParameters(IOrdinaryTypeSymbolNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedList<INamespaceMemberSymbolNode> NamespaceSymbol_Members(INamespaceSymbolNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ITypeMemberSymbolNode> ClassSymbol_Members(IClassSymbolNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ITypeMemberSymbolNode> StructSymbol_Members(IStructSymbolNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ITypeMemberSymbolNode> TraitSymbol_Members(ITraitSymbolNode node);
}
