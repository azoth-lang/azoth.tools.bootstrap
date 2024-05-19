using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Entities;

public class EntitySymbolBuilder
{
    private readonly ISymbolTreeBuilder symbolTree;
    private readonly SymbolForest symbolTrees;

    private EntitySymbolBuilder(ISymbolTreeBuilder symbolTree, SymbolForest symbolTrees)
    {
        this.symbolTree = symbolTree;
        this.symbolTrees = symbolTrees;
    }

    public static void BuildFor(PackageSyntax<Package> package)
    {
        var builder = new EntitySymbolBuilder(package.SymbolTree, package.SymbolTrees);
        builder.Build(package.EntityDeclarations);

        builder = new EntitySymbolBuilder(package.TestingSymbolTree, package.TestingSymbolTrees);
        builder.Build(package.TestingEntityDeclarations);
    }

    private void Build(IFixedSet<IEntityDefinitionSyntax> entities)
    {
        foreach (var entity in entities)
            BuildEntitySymbol(entity);

        var typeDeclarations = entities.OfType<ITypeDefinitionSyntax>();
        var inheritor = new TypeSymbolInheritor(symbolTree, symbolTrees, typeDeclarations);
        inheritor.AddInheritedSymbols();
    }

    private void BuildEntitySymbol(IEntityDefinitionSyntax entity)
    {
        switch (entity)
        {
            default:
                throw ExhaustiveMatch.Failed(entity);
            case IMethodDefinitionSyntax method:
                BuildMethodSymbol(method);
                break;
            case IConstructorDefinitionSyntax constructor:
                BuildConstructorSymbol(constructor);
                break;
            case IInitializerDefinitionSyntax initializer:
                BuildInitializerSymbol(initializer);
                break;
            case IAssociatedFunctionDefinitionSyntax associatedFunction:
                BuildAssociatedFunctionSymbol(associatedFunction);
                break;
            case IFieldDefinitionSyntax field:
                BuildFieldSymbol(field);
                break;
            case IFunctionDefinitionSyntax syn:
                BuildFunctionSymbol(syn);
                break;
            case IClassDefinitionSyntax @class:
                BuildClassSymbol(@class);
                break;
            case IStructDefinitionSyntax @struct:
                BuildStructSymbol(@struct);
                break;
            case ITraitDefinitionSyntax trait:
                BuildTraitSymbol(trait);
                break;
        }
    }

    private void BuildMethodSymbol(IMethodDefinitionSyntax method)
    {
        // Method symbol already set by EntitySymbolApplier
        var symbol = method.Symbol.Result;
        symbolTree.Add(symbol);

        // Parameter symbols already set by EntitySymbolApplier
        symbolTree.Add(method.SelfParameter.Symbol.Result);
        BuildParameterSymbols(method.Parameters);
    }

    private void BuildConstructorSymbol(IConstructorDefinitionSyntax constructor)
    {
        // Constructor symbol already set by EntitySymbolApplier
        var symbol = constructor.Symbol.Result;
        symbolTree.Add(symbol);

        // Parameter symbols already set by EntitySymbolApplier
        symbolTree.Add(constructor.SelfParameter.Symbol.Result);
        BuildParameterSymbols(constructor.Parameters);
    }

    private void BuildInitializerSymbol(IInitializerDefinitionSyntax initializer)
    {
        // Initializer symbol already set by EntitySymbolApplier
        var symbol = initializer.Symbol.Result;
        symbolTree.Add(symbol);

        // Parameter symbols already set by EntitySymbolApplier
        symbolTree.Add(initializer.SelfParameter.Symbol.Result);
        BuildParameterSymbols(initializer.Parameters);
    }

    private void BuildAssociatedFunctionSymbol(IAssociatedFunctionDefinitionSyntax associatedFunction)
    {
        // Associated function symbol already set by EntitySymbolApplier
        var symbol = associatedFunction.Symbol.Result;
        symbolTree.Add(symbol);

        // Parameter symbols already set by EntitySymbolApplier
        BuildParameterSymbols(associatedFunction.Parameters);
    }

    private void BuildFieldSymbol(IFieldDefinitionSyntax field)
    {
        // Field symbol already set by EntitySymbolApplier
        var symbol = field.Symbol.Result;
        symbolTree.Add(symbol);
    }

    private void BuildFunctionSymbol(IFunctionDefinitionSyntax function)
    {
        // Function symbol already set by EntitySymbolApplier
        var symbol = function.Symbol.Result;
        symbolTree.Add(symbol);

        // Parameter symbols already set by EntitySymbolApplier
        BuildParameterSymbols(function.Parameters);
    }

    private void BuildClassSymbol(IClassDefinitionSyntax @class)
    {
        // Class symbol already set by EntitySymbolApplier
        var classSymbol = @class.Symbol.Result;
        symbolTree.Add(classSymbol);

        // Generic parameter symbols already set by EntitySymbolApplier
        symbolTree.Add(@class.GenericParameters.Select(p => p.Symbol.Result));

        @class.CreateDefaultConstructor(symbolTree);
    }

    private void BuildStructSymbol(IStructDefinitionSyntax @struct)
    {
        // Struct symbol already set by EntitySymbolApplier
        var structSymbol = @struct.Symbol.Result;
        symbolTree.Add(structSymbol);

        // Generic parameter symbols already set by EntitySymbolApplier
        symbolTree.Add(@struct.GenericParameters.Select(p => p.Symbol.Result));

        @struct.CreateDefaultInitializer(symbolTree);
    }

    private void BuildTraitSymbol(ITraitDefinitionSyntax trait)
    {
        // Trait symbol already set by EntitySymbolApplier
        var traitSymbol = trait.Symbol.Result;
        symbolTree.Add(traitSymbol);

        // Generic parameter symbols already set by EntitySymbolApplier
        symbolTree.Add(trait.GenericParameters.Select(p => p.Symbol.Result));
    }

    private void BuildParameterSymbols(
        IEnumerable<IConstructorOrInitializerParameterSyntax> parameters)
    {
        foreach (var param in parameters)
        {
            switch (param)
            {
                default:
                    throw ExhaustiveMatch.Failed(param);
                case INamedParameterSyntax namedParam:
                    symbolTree.Add(namedParam.Symbol.Result);
                    break;
                case IFieldParameterSyntax _:
                    // Referenced field already assigned
                    break;
            }
        }
    }
}
