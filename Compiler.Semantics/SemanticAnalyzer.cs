using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Entities;
using Azoth.Tools.Bootstrap.Compiler.Semantics.SyntaxBinding;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Validation;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

public class SemanticAnalyzer
{
    /// <summary>
    /// Whether to store the reachability graphs for each function and method.
    /// Default Value: false
    /// </summary>
    public bool SaveReachabilityGraphs { get; set; }

    public IPackageNode Check(IPackageSyntax packageSyntax)
    {
        // If there are errors from the lex and parse phase, don't continue on
        packageSyntax.Diagnostics.ThrowIfFatalErrors();

        // Assign declaration numbers before building the semantic tree so that, for now, they can
        // be used to build symbols for the old syntax tree approach.
        //DeclarationNumberAssigner.AssignIn(packageSyntax.AllEntityDeclarations);

        // Build a semantic tree from the syntax tree
        var packageNode = SyntaxBinder.Bind(packageSyntax);

        // Check all semantic conditions
        CheckSemantics(packageNode);

        //// Run legacy semantic analysis of the package
        //LegacyBasicAnalysis(packageSyntax, packageNode);

        // If the semantic tree reports any fatal errors, don't continue on
        packageNode.Diagnostics.ThrowIfFatalErrors();

        return packageNode;
    }

    private static void CheckSemantics(IPackageNode package)
    {
#if DEBUG
        // Since the tree is lazy evaluated, walk it and force evaluation of many attributes to catch bugs
        SemanticTreeValidator.Validate(package);
#endif

        // TODO use DataFlowAnalysis to check for unused variables and report use of variables starting with `_`

        // If the semantic tree reports any fatal errors, don't continue on
        package.Diagnostics.ThrowIfFatalErrors();
    }

    private static void LegacyBasicAnalysis(IPackageSyntax packageSyntax, IPackageNode packageNode)
    {
        // Resolve symbols for the entities
        EntitySymbolBuilder.BuildFor(packageSyntax);

        // TODO handle `range` better
        var azothNamespaceSymbols = packageSyntax.SymbolTrees.GlobalSymbols
            .OfType<LocalNamespaceSymbol>().Where(s => s.Name == "azoth");
        var rangeSymbol = azothNamespaceSymbols.SelectMany(packageSyntax.SymbolTrees.Children)
            .OfType<UserTypeSymbol>().SingleOrDefault(s => s.Name == "range");

        // Basic Analysis includes: Name Binding, Type Checking, Constant Folding
        BasicAnalyzer.Check(packageSyntax, rangeSymbol);

        // Validate that types computed on the semantic tree match the types computed on the syntax tree
        SemanticTreeTypeValidator.Validate(packageNode);

        // If there are errors from the basic analysis phase, don't continue on
        // The syntax diagnostics are already included in the packageSyntax.Diagnostics
        packageSyntax.Diagnostics.Add(packageNode.Diagnostics.Except(packageSyntax.Diagnostics));
        packageSyntax.Diagnostics.ThrowIfFatalErrors();

    }
}
