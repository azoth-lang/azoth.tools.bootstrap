using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Entities;

public class EntitySymbolBuilder
{
    private readonly Diagnostics diagnostics;
    private readonly ISymbolTreeBuilder symbolTree;
    private readonly SymbolForest symbolTrees;

    private EntitySymbolBuilder(Diagnostics diagnostics, ISymbolTreeBuilder symbolTree, SymbolForest symbolTrees)
    {
        this.diagnostics = diagnostics;
        this.symbolTree = symbolTree;
        this.symbolTrees = symbolTrees;
    }

    public static void BuildFor(PackageSyntax<Package> package)
    {
        var builder = new EntitySymbolBuilder(package.Diagnostics, package.SymbolTree, package.SymbolTrees);
        builder.Build(package.EntityDeclarations);

        builder = new EntitySymbolBuilder(package.Diagnostics, package.TestingSymbolTree, package.TestingSymbolTrees);
        builder.Build(package.TestingEntityDeclarations);
    }

    private void Build(IFixedSet<IEntityDeclarationSyntax> entities)
    {
        // Process all types first because they may be referenced by functions etc.
        var typeDeclarations = entities.OfType<ITypeDeclarationSyntax>().ToFixedList();
        foreach (var type in typeDeclarations)
            BuildTypeSymbol(type);

        // Now resolve all other symbols (type declarations will already have symbols and won't be processed again)
        foreach (var entity in entities)
            BuildNonTypeEntitySymbol(entity);

        var inheritor = new TypeSymbolInheritor(symbolTree, symbolTrees, typeDeclarations);
        inheritor.AddInheritedSymbols();
    }

    /// <summary>
    /// If the type has not been resolved, this resolves it. This function
    /// also watches for type cycles and reports an error.
    /// </summary>
    private void BuildNonTypeEntitySymbol(IEntityDeclarationSyntax entity)
    {
        switch (entity)
        {
            default:
                throw ExhaustiveMatch.Failed(entity);
            case IMethodDeclarationSyntax method:
                BuildMethodSymbol(method);
                break;
            case IConstructorDeclarationSyntax constructor:
                BuildConstructorSymbol(constructor);
                break;
            case IInitializerDeclarationSyntax initializer:
                BuildInitializerSymbol(initializer);
                break;
            case IAssociatedFunctionDeclarationSyntax associatedFunction:
                BuildAssociatedFunctionSymbol(associatedFunction);
                break;
            case IFieldDeclarationSyntax field:
                BuildFieldSymbol(field);
                break;
            case IFunctionDeclarationSyntax syn:
                BuildFunctionSymbol(syn);
                break;
            case ITypeDeclarationSyntax _:
                // Type declarations already processed
                break;
        }
    }

    private void BuildMethodSymbol(IMethodDeclarationSyntax method)
    {
        // Method symbol already set by EntitySymbolApplier
        var symbol = method.Symbol.Result;
        symbolTree.Add(symbol);

        // EntitySymbolApplier doesn't cover parameters because they are not metadata symbols
        var selfParameterType = symbol.SelfParameterType.Type;
        var parameterTypes = symbol.MethodGroupType.Parameters;
        BuildSelfParameterSymbol(symbol, method.SelfParameter, selfParameterType);
        BuildParameterSymbols(symbol, method.File, method.Parameters, parameterTypes);
    }

    private void BuildConstructorSymbol(IConstructorDeclarationSyntax constructor)
    {
        // Constructor symbol already set by EntitySymbolApplier
        var symbol = constructor.Symbol.Result;
        symbolTree.Add(symbol);

        // EntitySymbolApplier doesn't cover parameters because they are not metadata symbols
        var selfParameterType = symbol.SelfParameterType;
        var parameterTypes = symbol.Parameters;
        BuildSelfParameterSymbol(symbol, constructor.SelfParameter, selfParameterType, isConstructor: true);
        BuildParameterSymbols(symbol, constructor.File, constructor.Parameters, parameterTypes);
    }

    private void BuildInitializerSymbol(IInitializerDeclarationSyntax initializer)
    {
        // Initializer symbol already set by EntitySymbolApplier
        var symbol = initializer.Symbol.Result;
        symbolTree.Add(symbol);

        // EntitySymbolApplier doesn't cover parameters because they are not metadata symbols
        var selfParameterType = symbol.SelfParameterType;
        var parameterTypes = symbol.Parameters;
        BuildSelfParameterSymbol(symbol, initializer.SelfParameter, selfParameterType, isConstructor: true);
        BuildParameterSymbols(symbol, initializer.File, initializer.Parameters, parameterTypes);
    }

    private void BuildAssociatedFunctionSymbol(IAssociatedFunctionDeclarationSyntax associatedFunction)
    {
        // Associated function symbol already set by EntitySymbolApplier
        var symbol = associatedFunction.Symbol.Result;
        symbolTree.Add(symbol);

        // EntitySymbolApplier doesn't cover parameters because they are not metadata symbols
        var parameterTypes = symbol.Type.Parameters.Select(p => p.Type);
        BuildParameterSymbols(symbol, associatedFunction.File, associatedFunction.Parameters, parameterTypes);
    }

    private void BuildFieldSymbol(IFieldDeclarationSyntax field)
    {
        // Field symbol already set by EntitySymbolApplier
        var symbol = field.Symbol.Result;
        symbolTree.Add(symbol);
    }

    private void BuildFunctionSymbol(IFunctionDeclarationSyntax function)
    {
        // Function symbol already set by EntitySymbolApplier
        var symbol = function.Symbol.Result;
        symbolTree.Add(symbol);

        // EntitySymbolApplier doesn't cover parameters because they are not metadata symbols
        var parameterTypes = symbol.Type.Parameters.Select(p => p.Type);
        BuildParameterSymbols(symbol, function.File, function.Parameters, parameterTypes);
    }

    private void BuildTypeSymbol(ITypeDeclarationSyntax type)
    {
        switch (type)
        {
            default:
                throw ExhaustiveMatch.Failed(type);
            case IClassDeclarationSyntax @class:
                BuildClassSymbol(@class);
                break;
            case IStructDeclarationSyntax @struct:
                BuildStructSymbol(@struct);
                break;
            case ITraitDeclarationSyntax trait:
                BuildTraitSymbol(trait);
                break;
        }
    }

    private void BuildClassSymbol(IClassDeclarationSyntax @class)
    {
        // Class symbol already set by EntitySymbolApplier
        var classSymbol = @class.Symbol.Result;
        symbolTree.Add(classSymbol);

        // Generic parameter symbols already set by EntitySymbolApplier
        symbolTree.Add(@class.GenericParameters.Select(p => p.Symbol.Result));

        @class.CreateDefaultConstructor(symbolTree);
    }

    private void BuildStructSymbol(IStructDeclarationSyntax @struct)
    {
        // Struct symbol already set by EntitySymbolApplier
        var structSymbol = @struct.Symbol.Result;
        symbolTree.Add(structSymbol);

        // Generic parameter symbols already set by EntitySymbolApplier
        symbolTree.Add(@struct.GenericParameters.Select(p => p.Symbol.Result));

        @struct.CreateDefaultInitializer(symbolTree);
    }

    private void BuildTraitSymbol(ITraitDeclarationSyntax trait)
    {
        // Trait symbol already set by EntitySymbolApplier
        var traitSymbol = trait.Symbol.Result;
        symbolTree.Add(traitSymbol);

        // Generic parameter symbols already set by EntitySymbolApplier
        symbolTree.Add(trait.GenericParameters.Select(p => p.Symbol.Result));
    }

    private void BuildParameterSymbols(
        InvocableSymbol containingSymbol,
        CodeFile file,
        IEnumerable<IConstructorOrInitializerParameterSyntax> parameters,
        IEnumerable<Parameter> parameterTypes)
        => BuildParameterSymbols(containingSymbol, file, parameters, parameterTypes.Select(pt => pt.Type));

    private void BuildParameterSymbols(
        InvocableSymbol containingSymbol,
        CodeFile file,
        IEnumerable<IConstructorOrInitializerParameterSyntax> parameters,
        IEnumerable<DataType> types)
    {
        foreach (var (param, type) in parameters.EquiZip(types))
        {
            switch (param)
            {
                default:
                    throw ExhaustiveMatch.Failed(param);
                case INamedParameterSyntax namedParam:
                {
                    var isLent = namedParam.IsLentBinding;
                    if (isLent && !type.CanBeLent())
                    {
                        diagnostics.Add(TypeError.TypeCannotBeLent(file, namedParam.Span, type));
                        isLent = false;
                    }

                    var symbol = NamedVariableSymbol.CreateParameter(containingSymbol, namedParam.Name,
                        namedParam.DeclarationNumber.Result, namedParam.IsMutableBinding, isLent, type);
                    namedParam.Symbol.Fulfill(symbol);
                    symbolTree.Add(symbol);
                }
                break;
                case IFieldParameterSyntax _:
                    // Referenced field already assigned
                    break;
            }
        }
    }

    private void BuildSelfParameterSymbol(
        InvocableSymbol containingSymbol,
        ISelfParameterSyntax param,
        Pseudotype type,
        bool isConstructor = false)
    {
        var symbol = new SelfParameterSymbol(containingSymbol, param.IsLentBinding && !isConstructor, type);
        param.Symbol.Fulfill(symbol);
        symbolTree.Add(symbol);
    }
}
