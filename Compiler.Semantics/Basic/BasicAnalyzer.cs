using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
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
/// * Infer: infer what type something has
/// * Evaluate: determine the type for some type syntax
/// </summary>
public class BasicAnalyzer
{
    private readonly SymbolTreeBuilder symbolTreeBuilder;
    private readonly SymbolForest symbolTrees;
    private readonly ObjectTypeSymbol? stringSymbol;
    private readonly Diagnostics diagnostics;

    private BasicAnalyzer(
        SymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        ObjectTypeSymbol? stringSymbol,
        Diagnostics diagnostics)
    {
        this.symbolTreeBuilder = symbolTreeBuilder;
        this.symbolTrees = symbolTrees;
        this.stringSymbol = stringSymbol;
        this.diagnostics = diagnostics;
    }

    public static void Check(PackageSyntax<Package> package, ObjectTypeSymbol? stringSymbol)
    {
        var analyzer = new BasicAnalyzer(package.SymbolTree, package.SymbolTrees, stringSymbol, package.Diagnostics);
        analyzer.Check(package.AllEntityDeclarations);
    }

    private void Check(FixedSet<IEntityDeclarationSyntax> entities)
    {
        foreach (var entity in entities)
            ResolveBodyTypes(entity);
    }

    private void ResolveBodyTypes(IEntityDeclarationSyntax declaration)
    {
        switch (declaration)
        {
            default:
                throw ExhaustiveMatch.Failed(declaration);
            case IFunctionDeclarationSyntax function:
            {
                var resolver = new BasicBodyAnalyzer(function, symbolTreeBuilder, symbolTrees, stringSymbol, diagnostics,
                    function.Symbol.Result.ReturnDataType);
                resolver.ResolveTypes(function.Body);
                break;
            }
            case IAssociatedFunctionDeclarationSyntax associatedFunction:
            {
                var resolver = new BasicBodyAnalyzer(associatedFunction, symbolTreeBuilder, symbolTrees,
                    stringSymbol, diagnostics,
                    associatedFunction.Symbol.Result.ReturnDataType);
                resolver.ResolveTypes(associatedFunction.Body);
                break;
            }
            case IConcreteMethodDeclarationSyntax method:
            {
                var resolver = new BasicBodyAnalyzer(method, symbolTreeBuilder,
                    symbolTrees, stringSymbol, diagnostics, method.Symbol.Result.ReturnDataType);
                resolver.ResolveTypes(method.Body);
                break;
            }
            case IAbstractMethodDeclarationSyntax _:
                // has no body, so nothing to resolve
                break;
            case IFieldDeclarationSyntax field:
                if (field.Initializer != null)
                {
                    var resolver = new BasicBodyAnalyzer(field, symbolTreeBuilder, symbolTrees, stringSymbol, diagnostics);
                    resolver.CheckType(field.Initializer, field.Symbol.Result.DataType, new SharingRelation(), new ReferenceCapabilities());
                }
                break;
            case IConstructorDeclarationSyntax constructor:
            {
                var resolver = new BasicBodyAnalyzer(constructor, symbolTreeBuilder, symbolTrees, stringSymbol, diagnostics, constructor.ImplicitSelfParameter.Symbol.Result.DataType);
                resolver.ResolveTypes(constructor.Body);
                break;
            }
            case IClassDeclarationSyntax _:
                // body of class is processed as separate items
                break;
        }
    }
}
