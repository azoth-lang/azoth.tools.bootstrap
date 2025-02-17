using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal static partial class SymbolsAspect
{
    #region Packages
    public static partial PackageSymbol Package_Symbol(IPackageNode node) => new(node.Name);

    /// <summary>
    /// All the symbols of this package in a form suitable for other packages to reference.
    /// </summary>
    public static partial IPackageSymbols Package_PackageSymbols(IPackageNode node)
    {
        var mainTreeBuilder = new SymbolTreeBuilder(node.Symbol);
        var mainForest = BuiltIn.CreateSymbolForest(mainTreeBuilder, node.References.Select(p => p.PackageSymbols.SymbolTree));
        var mainTree = new PackageSymbolTreeBuilder(mainTreeBuilder, node.MainFacet, mainForest).Build();
        var testingTreeBuilder = new TestingSymbolTreeBuilder(mainTreeBuilder);
        var testingForest = BuiltIn.CreateSymbolForest(testingTreeBuilder, node.References.Select(p => p.PackageSymbols.TestingSymbolTree));
        var testingTree = new PackageSymbolTreeBuilder(testingTreeBuilder, node.MainFacet, testingForest).Build();
        return new PackageSymbols(node.Symbol, mainTree, testingTree);
    }
    #endregion

    #region Namespace Definitions
    public static partial NamespaceSymbol NamespaceDefinition_Symbol(INamespaceDefinitionNode node)
    {
        if (node.Name is { } name)
        {
            var containingSymbol = (NamespaceSymbol)node.ContainingDeclaration.Symbol!;
            return new LocalNamespaceSymbol(containingSymbol, name);
        }

        return node.Package.Symbol;
    }

    #endregion

    #region Function Definition
    public static partial FunctionSymbol? FunctionDefinition_Symbol(IFunctionDefinitionNode node)
    {
        if (node.Type is not FunctionType type) return null;
        return new(node.ContainingSymbol, node.Name, type);
    }
    #endregion

    #region Type Definitions
    public static partial OrdinaryTypeSymbol TypeDefinition_Symbol(ITypeDefinitionNode node)
        => new(node.ContainingSymbol, node.TypeConstructor);
    #endregion

    #region Type Definition Parts
    public static partial GenericParameterTypeSymbol GenericParameter_Symbol(IGenericParameterNode node)
        => new(node.ContainingSymbol, node.DeclaredType.PlainType);

    public static partial AssociatedTypeSymbol ImplicitSelfDefinition_Symbol(IImplicitSelfDefinitionNode node)
        => new AssociatedTypeSymbol(node.ContainingSymbol, node.TypeConstructor);
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
        return new(node.ContainingSymbol, node.Kind, node.Name, selfParameterType, parameters, returnType);
    }

    public static partial InitializerSymbol? OrdinaryInitializerDefinition_Symbol(IOrdinaryInitializerDefinitionNode node)
    {
        if (node.ParameterTypes.AsKnownFixedList() is not { } parameters) return null;
        return new(node.ContainingSymbol, node.Name, node.SelfParameter.BindingType, parameters);
    }

    public static partial InitializerSymbol? DefaultInitializerDefinition_Symbol(IDefaultInitializerDefinitionNode node)
        => InitializerSymbol.CreateDefault(node.ContainingSymbol);

    public static partial FieldSymbol? FieldDefinition_Symbol(IFieldDefinitionNode node)
    {
        if (node.BindingType is not NonVoidType bindingType) return null;
        return new(node.ContainingSymbol, node.IsMutableBinding, node.Name, bindingType);
    }

    public static partial FunctionSymbol? AssociatedFunctionDefinition_Symbol(IAssociatedFunctionDefinitionNode node)
    {
        if (node.Type is not FunctionType type)
            return null;
        return new(node.ContainingSymbol, node.Name, type);
    }

    public static partial AssociatedTypeSymbol AssociatedTypeDefinition_Symbol(IAssociatedTypeDefinitionNode node)
        => new(node.ContainingSymbol, node.TypeConstructor);
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
