using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DeclarationNumbers;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Liveness;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Entities;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Namespaces;
using Azoth.Tools.Bootstrap.Compiler.Semantics.SyntaxBinding;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Validation;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables.BindingMutability;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables.DefiniteAssignment;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables.Shadowing;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

public class SemanticAnalyzer
{
    /// <summary>
    /// Whether to store the liveness analysis for each function and method.
    /// Default Value: false
    /// </summary>
    public bool SaveLivenessAnalysis { get; set; }

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
        DeclarationNumberAssigner.AssignIn(packageSyntax.AllEntityDeclarations);

        // New semantic tree approach
        var packageNode = BuildSemanticTreeAndValidate(packageSyntax);

        // Apply symbols from the semantic tree to the old syntax tree approach
        SemanticsApplier.Apply(packageNode);

        // Load namespace symbols applied to the old syntax tree approach into the symbol trees
        NamespaceSymbolCollector.Collect(packageNode, packageSyntax.SymbolTree, packageSyntax.TestingSymbolTree);

        // Check the semantics of the package
        var packageBuilder = CheckSemantics(packageSyntax, packageNode);

        // If there are errors from the semantics phase, don't continue on
        packageBuilder.Diagnostics.ThrowIfFatalErrors();

        // TODO remove hack once all diagnostics are generated from the semantic tree
        packageNode.AddDistinctDiagnostics(packageBuilder.Diagnostics);

        return packageNode;
    }

    private static IPackageNode BuildSemanticTreeAndValidate(IPackageSyntax packageSyntax)
    {
        // Start of new attribute grammar based approach
        var packageNode = SyntaxBinder.Bind(packageSyntax);

        // Since the tree is lazy evaluated, walk it and force evaluation of many attributes to catch bugs
        SemanticTreeValidator.Validate(packageNode);

        // If the semantic tree reports any fatal errors, don't continue on
        // TODO add back once old semantic checks are removed
        //packageNode.Diagnostics.ThrowIfFatalErrors();

        return packageNode;
    }

    private static PackageBuilder CheckSemantics(IPackageSyntax packageSyntax, IPackageNode packageNode)
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

#if DEBUG
        // Validate various properties of the package before continuing. Helps find bugs in the
        // analysis up to this point and avoids confusing error messages from later stages.
        new SymbolValidator(packageSyntax.SymbolTree).Validate(packageSyntax.EntityDeclarations);
        new SymbolValidator(packageSyntax.TestingSymbolTree).Validate(packageSyntax.TestingEntityDeclarations);
        new SemanticsFulfillmentValidator().Validate(packageSyntax.AllEntityDeclarations);
        new TypeFulfillmentValidator().Validate(packageSyntax.AllEntityDeclarations);
        new TypeKnownValidator().Validate(packageSyntax.AllEntityDeclarations);
#endif

        var packageBuilder = new ASTBuilder().BuildPackage(packageSyntax);

        CheckDataFlow(packageBuilder.Declarations, packageBuilder.SymbolTree, packageBuilder.Diagnostics);
        CheckDataFlow(packageBuilder.TestingDeclarations, packageBuilder.TestingSymbolTree, packageBuilder.Diagnostics);

        return packageBuilder;
    }

    private static void CheckDataFlow(IFixedSet<IDeclaration> declarations, FixedSymbolTree symbolTree, Diagnostics diagnostics)
    {
        // From this point forward, analysis focuses on executable declarations (i.e. invocables and field initializers)
        var executableDeclarations = declarations.OfType<IExecutableDeclaration>().ToFixedSet();

        ShadowChecker.Check(executableDeclarations, diagnostics);

        DataFlowAnalysis.Check(DefiniteAssignmentAnalyzer.Instance, executableDeclarations, symbolTree,
            diagnostics);

        DataFlowAnalysis.Check(BindingMutabilityAnalyzer.Instance, executableDeclarations, symbolTree,
            diagnostics);

        // TODO use DataFlowAnalysis to check for unused variables and report use of variables starting with `_`

        // Compute variable liveness needed by reachability analyzer
        DataFlowAnalysis.Check(LivenessAnalyzer.Instance, executableDeclarations, symbolTree,
            diagnostics);

        // TODO remove live variables if SaveLivenessAnalysis is false
    }
}
