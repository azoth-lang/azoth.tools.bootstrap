using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal static partial class SymbolNodeAspect
{
    public static partial IPackageFacetSymbolNode PackageFacetReference_SymbolNode(IPackageFacetReferenceNode node)
        => IPackageFacetSymbolNode.Create(node.Symbols);

    private static IEnumerable<Symbol> GetSymbolMembers(IChildSymbolNode node)
        => node.SymbolTree().GetChildrenOf(node.Symbol);

    #region Facet Symbol Nodes
    public static partial INamespaceSymbolNode PackageFacetSymbol_GlobalNamespace(IPackageFacetSymbolNode node)
        => INamespaceSymbolNode.Create(node.SymbolTree.Facet);
    #endregion

    #region Namespace Symbol Nodes
    public static partial IFixedList<INamespaceMemberSymbolNode> NamespaceSymbol_Members(INamespaceSymbolNode node)
        => GetSymbolMembers(node).Select(SymbolBinder.Symbol).Cast<INamespaceMemberSymbolNode>().ToFixedList();
    #endregion

    #region Type Symbol Nodes
    public static partial ISelfSymbolNode NonVariableTypeSymbol_ImplicitSelf(INonVariableTypeSymbolNode node)
        => ISelfSymbolNode.Create(node.SymbolTree().GetChildrenOf(node.Symbol).OfType<AssociatedTypeSymbol>().Where(t => t.Name == BuiltInTypeName.Self).TrySingle()!);

    public static partial IFixedList<IGenericParameterSymbolNode> OrdinaryTypeSymbol_GenericParameters(IOrdinaryTypeSymbolNode node)
        => GetSymbolMembers(node).OfType<GenericParameterTypeSymbol>()
                           .Select(SymbolBinder.Symbol).WhereNotNull()
                           .Cast<IGenericParameterSymbolNode>().ToFixedList();

    public static partial IFixedSet<ITypeMemberSymbolNode> BuiltInTypeSymbol_Members(IBuiltInTypeSymbolNode node)
        => GetMembers(node);

    public static partial void Validate_ClassSymbol(OrdinaryTypeSymbol symbol)
        => Requires.That(symbol.TypeConstructor is { Kind: TypeKind.Class }, nameof(symbol),
            "Symbol must be for an class type.");

    public static partial IFixedSet<ITypeMemberSymbolNode> ClassSymbol_Members(IClassSymbolNode node)
        => GetMembers(node);

    public static partial void Validate_StructSymbol(OrdinaryTypeSymbol symbol)
        => Requires.That(symbol.TypeConstructor is { Kind: TypeKind.Struct }, nameof(symbol),
            "Symbol must be for a struct type.");

    public static partial IFixedSet<ITypeMemberSymbolNode> StructSymbol_Members(IStructSymbolNode node)
        => GetMembers(node);

    public static partial void Validate_TraitSymbol(OrdinaryTypeSymbol symbol)
        => Requires.That(symbol.TypeConstructor is { Kind: TypeKind.Trait }, nameof(symbol),
            "Symbol must be for an trait type.");

    public static partial IFixedSet<ITypeMemberSymbolNode> TraitSymbol_Members(ITraitSymbolNode node)
        => GetMembers(node);

    private static IFixedSet<ITypeMemberSymbolNode> GetMembers(ITypeSymbolNode node)
        => GetSymbolMembers(node).Where(sym => sym is not GenericParameterTypeSymbol)
                           .Select(SymbolBinder.Symbol).WhereNotNull()
                           .OfType<ITypeMemberSymbolNode>().ToFixedSet();
    #endregion

    #region Member Symbol Nodes
    public static partial void Validate_OrdinaryMethodSymbol(MethodSymbol symbol)
        => Requires.That(symbol.Kind == MethodKind.Standard, nameof(symbol),
            "Must be a standard method symbol.");

    public static partial void Validate_GetterMethodSymbol(MethodSymbol symbol)
        => Requires.That(symbol.Kind == MethodKind.Getter, nameof(symbol), "Must be a getter symbol.");

    public static partial void Validate_SetterMethodSymbol(MethodSymbol symbol)
        => Requires.That(symbol.Kind == MethodKind.Setter, nameof(symbol), "Must be a standard method symbol.");
    #endregion
}
