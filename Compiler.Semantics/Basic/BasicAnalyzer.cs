using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic;

/// <summary>
/// The basic analyzer does name binding, type checking and constant folding
/// within statements and expressions. Evaluating parameter and return types
/// has already been completed as part of symbol binding. This class handles
/// declarations and delegates expressions, types etc. to other classes.
///
/// All basic analysis uses specific terminology to distinguish different
/// aspects of type checking. (The entry method `Check` is an exception. It
/// is named to match other analyzers but performs a resolve.)
///
/// Terminology:
///
/// * Resolve: includes type inference and checking
/// * Check: check something has an expected type
/// * Infer: infer what type (or sometimes symbol) something has
/// * Evaluate: determine the type for some type syntax
/// </summary>
public class BasicAnalyzer
{
    private readonly ISymbolTreeBuilder symbolTreeBuilder;
    private readonly SymbolForest symbolTrees;
    private readonly UserTypeSymbol? rangeSymbol;
    private readonly Diagnostics diagnostics;

    private BasicAnalyzer(
        ISymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        UserTypeSymbol? rangeSymbol,
        Diagnostics diagnostics)
    {
        this.symbolTreeBuilder = symbolTreeBuilder;
        this.symbolTrees = symbolTrees;
        this.rangeSymbol = rangeSymbol;
        this.diagnostics = diagnostics;
    }

    public static void Check(PackageSyntax<Package> package, UserTypeSymbol? rangeSymbol)
    {
        // Analyze standard code (*.az)
        var analyzer = new BasicAnalyzer(package.SymbolTree, package.SymbolTrees, rangeSymbol, package.Diagnostics);
        analyzer.Resolve(package.EntityDeclarations);

        // Analyze test code (*.azt)
        analyzer = new BasicAnalyzer(package.TestingSymbolTree, package.TestingSymbolTrees, rangeSymbol, package.Diagnostics);
        analyzer.Resolve(package.TestingEntityDeclarations);
    }

    private void Resolve(IFixedSet<IEntityDefinitionSyntax> entities)
    {
        foreach (var entity in entities)
            Resolve(entity);
    }

    private void Resolve(IEntityDefinitionSyntax entity)
    {
        switch (entity)
        {
            default:
                throw ExhaustiveMatch.Failed(entity);
            case ITypeDefinitionSyntax _:
                // Nothing to check
                break;
            case IInvocableDefinitionSyntax syn:
                ResolveBody(syn);
                break;
            case IFieldDefinitionSyntax syn:
                Resolve(syn);
                break;
        }
    }

    private void ResolveBody(IInvocableDefinitionSyntax definition)
    {
        switch (definition)
        {
            default:
                throw ExhaustiveMatch.Failed(definition);
            case IFunctionDefinitionSyntax function:
                {
                    var resolver = new BasicBodyAnalyzer(function, symbolTreeBuilder, symbolTrees,
                        rangeSymbol, diagnostics,
                        function.Symbol.Result.Return);
                    resolver.ResolveTypes(function.Body);
                    break;
                }
            case IAssociatedFunctionDefinitionSyntax associatedFunction:
                {
                    var resolver = new BasicBodyAnalyzer(associatedFunction, symbolTreeBuilder, symbolTrees,
                        rangeSymbol, diagnostics,
                        associatedFunction.Symbol.Result.Return);
                    resolver.ResolveTypes(associatedFunction.Body);
                    break;
                }
            case IConcreteMethodDefinitionSyntax method:
                {
                    var resolver = new BasicBodyAnalyzer(method, symbolTreeBuilder,
                        symbolTrees, rangeSymbol, diagnostics,
                        method.Symbol.Result.Return);
                    resolver.ResolveTypes(method.Body);
                    break;
                }
            case IAbstractMethodDefinitionSyntax _:
                // has no body, so nothing to resolve
                break;
            case IConstructorDefinitionSyntax constructor:
                {
                    Return @return = new Return(constructor.SelfParameter.DataType.Result);
                    var resolver = new BasicBodyAnalyzer(constructor, symbolTreeBuilder, symbolTrees,
                        rangeSymbol, diagnostics, @return);
                    resolver.ResolveTypes(constructor.Body);
                    break;
                }
            case IInitializerDefinitionSyntax initializer:
                {
                    Return @return = new Return(initializer.SelfParameter.DataType.Result);
                    var resolver = new BasicBodyAnalyzer(initializer, symbolTreeBuilder, symbolTrees,
                        rangeSymbol, diagnostics, @return);
                    resolver.ResolveTypes(initializer.Body);
                    break;
                }
        }
    }

    private void Resolve(IFieldDefinitionSyntax field)
    {
        if (field.Initializer is null)
            return;

        var resolver = new BasicBodyAnalyzer(field, symbolTreeBuilder, symbolTrees, rangeSymbol, diagnostics);
        resolver.CheckFieldInitializerType(field.Initializer, field.Symbol.Result.Type);
    }
}
