using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal static partial class SymbolsAspect
{
    #region Facets
    public static partial PackageFacetSymbol PackageFacet_Symbol(IPackageFacetNode node)
        => new(node.PackageSymbol, node.Kind);

    public static partial FixedSymbolTree PackageFacet_Symbols(IPackageFacetNode node)
    {
        var treeBuilder = new SymbolTreeBuilder(node.Symbol);
        var forest = BuiltIn.CreateSymbolForest(treeBuilder, node.AllReferences.Select(p => p.Symbols));
        return new PackageSymbolTreeBuilder(treeBuilder, node, forest).Build();
    }
    #endregion

    #region Namespace Definitions
    public static partial NamespaceSymbol NamespaceDefinition_Symbol(INamespaceDefinitionNode node)
    {
        if (node.Name is { } name)
            return new LocalNamespaceSymbol(node.ContainingSymbol(), name);

        // The containing symbol ought to be the package facet symbol
        // TODO is this correct? It used to be PackageSymbol
        return node.ContainingSymbol();
    }
    #endregion

    #region Function Definition
    public static partial FunctionSymbol? FunctionDefinition_Symbol(IFunctionDefinitionNode node)
    {
        if (node.Type is not FunctionType type) return null;
        return new(node.ContainingSymbol(), node.Name, type);
    }
    #endregion

    #region Type Definitions
    public static partial OrdinaryTypeSymbol TypeDefinition_Symbol(ITypeDefinitionNode node)
        => new(node.ContainingSymbol(), node.TypeConstructor);
    #endregion

    #region Type Definition Parts
    public static partial GenericParameterTypeSymbol GenericParameter_Symbol(IGenericParameterNode node)
        => new(node.ContainingSymbol(), node.DeclaredType.PlainType);

    public static partial AssociatedTypeSymbol ImplicitSelfDefinition_Symbol(IImplicitSelfDefinitionNode node)
        => new(node.ContainingSymbol(), node.TypeConstructor);
    #endregion

    #region Member Definitions
    public static partial MethodSymbol? MethodDefinition_Symbol(IMethodDefinitionNode node)
    {
        if (node.SelfParameterType is not NonVoidType selfParameterType)
            return null;
        if (node.ReturnType is not Type returnType)
            return null;
        if (node.ParameterTypes.AsKnownFixedList() is not { } parameters)
            return null;
        return new(node.ContainingSymbol(), node.Kind, node.Name, selfParameterType, parameters, returnType);
    }

    public static partial InitializerSymbol? OrdinaryInitializerDefinition_Symbol(IOrdinaryInitializerDefinitionNode node)
    {
        if (node.ParameterTypes.AsKnownFixedList() is not { } parameters) return null;
        return new(node.ContainingSymbol(), node.Name, node.SelfParameter.BindingType, parameters);
    }

    public static partial InitializerSymbol? DefaultInitializerDefinition_Symbol(IDefaultInitializerDefinitionNode node)
        => InitializerSymbol.CreateDefault(node.ContainingSymbol());

    public static partial FieldSymbol? FieldDefinition_Symbol(IFieldDefinitionNode node)
    {
        if (node.BindingType is not NonVoidType bindingType) return null;
        return new(node.ContainingSymbol(), node.IsMutableBinding, node.Name, bindingType);
    }

    public static partial FunctionSymbol? AssociatedFunctionDefinition_Symbol(IAssociatedFunctionDefinitionNode node)
    {
        if (node.Type is not FunctionType type)
            return null;
        return new(node.ContainingSymbol(), node.Name, type);
    }

    public static partial AssociatedTypeSymbol AssociatedTypeDefinition_Symbol(IAssociatedTypeDefinitionNode node)
        => new(node.ContainingSymbol(), node.TypeConstructor);
    #endregion

    #region Attributes
    // TODO eliminate ReferencedSymbol because ReferencedDeclaration should be used instead
    public static partial InvocableSymbol? Attribute_ReferencedSymbol(IAttributeNode node)
    {
        var referencedTypeSymbolNode = node.TypeName.ReferencedDeclaration;
        if (referencedTypeSymbolNode is not IOrdinaryTypeDeclarationNode userTypeSymbolNode)
            return null;

        var symbol = userTypeSymbolNode.InclusiveMembers.OfType<IInitializerDeclarationNode>()
                                       .Where(c => c.ParameterTypes.IsEmpty)
                                       .Select(c => c.Symbol)
                                       .SingleOrDefault();
        return symbol;
    }
    #endregion
}
