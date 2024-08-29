using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal static partial class SymbolsAspect
{
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

    public static partial UserTypeSymbol TypeDefinition_Symbol(ITypeDefinitionNode node)
        => new(node.ContainingSymbol, node.DeclaredType);

    public static partial GenericParameterTypeSymbol GenericParameter_Symbol(IGenericParameterNode node)
        => new(node.ContainingSymbol, node.DeclaredType);

    public static partial TypeSymbol? StandardTypeName_ReferencedSymbol(IStandardTypeNameNode node)
        => node.ReferencedDeclaration?.Symbol;

    public static partial TypeSymbol? QualifiedTypeName_ReferencedSymbol(IQualifiedTypeNameNode node)
        => throw new NotImplementedException();

    public static partial TypeSymbol SpecialTypeName_ReferencedSymbol(ISpecialTypeNameNode node)
        => Primitive.SymbolTree.LookupSymbol(node.Name);

    public static partial FunctionSymbol FunctionDefinition_Symbol(IFunctionDefinitionNode node)
        => new(node.ContainingSymbol, node.Name, node.Type);

    #region Member Definitions
    public static partial MethodSymbol MethodDefinition_Symbol(IMethodDefinitionNode node)
        => new(node.ContainingSymbol, node.Kind, node.Name,
            node.SelfParameter.ParameterType,
            node.Parameters.Select(p => p.ParameterType).ToFixedList(),
            new(node.Return?.NamedType ?? DataType.Void));

    public static partial ConstructorSymbol SourceConstructorDefinition_Symbol(ISourceConstructorDefinitionNode node)
        => new(node.ContainingSymbol, node.Name, node.SelfParameter.BindingType,
            node.Parameters.Select(p => p.ParameterType).ToFixedList());

    public static partial ConstructorSymbol DefaultConstructorDefinition_Symbol(IDefaultConstructorDefinitionNode node)
        => ConstructorSymbol.CreateDefault(node.ContainingSymbol);

    public static partial InitializerSymbol SourceInitializerDefinition_Symbol(ISourceInitializerDefinitionNode node)
        => new(node.ContainingSymbol, node.Name, node.SelfParameter.BindingType,
            node.Parameters.Select(p => p.ParameterType).ToFixedList());

    public static partial InitializerSymbol DefaultInitializerDefinition_Symbol(IDefaultInitializerDefinitionNode node)
        => InitializerSymbol.CreateDefault(node.ContainingSymbol);

    public static partial FieldSymbol FieldDefinition_Symbol(IFieldDefinitionNode node)
        => new(node.ContainingSymbol, node.Name, node.IsMutableBinding, node.BindingType);

    public static partial FunctionSymbol AssociatedFunctionDefinition_Symbol(IAssociatedFunctionDefinitionNode node)
        => new(node.ContainingSymbol, node.Name, node.Type);
    #endregion

    #region Attributes
    public static partial ConstructorSymbol? Attribute_ReferencedSymbol(IAttributeNode node)
    {
        var referencedTypeSymbolNode = node.TypeName.ReferencedDeclaration;
        if (referencedTypeSymbolNode is not IUserTypeDeclarationNode userTypeSymbolNode)
            return null;

        return userTypeSymbolNode.Members.OfType<IConstructorDeclarationNode>().Select(c => c.Symbol)
                                 .SingleOrDefault(s => s.Arity == 0);
    }
    #endregion
}