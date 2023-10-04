using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.AST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DeclarationNumbers;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Liveness;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Startup;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Entities;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Namespaces;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Validation;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables.BindingMutability;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables.DefiniteAssignment;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables.Moves;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables.Shadowing;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
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

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
    public Package Check(PackageSyntax<Package> packageSyntax)
    {
        // If there are errors from the lex and parse phase, don't continue on
        packageSyntax.Diagnostics.ThrowIfFatalErrors();

        NamespaceSymbolBuilder.BuildNamespaceSymbols(packageSyntax);

        // Build up lexical scopes down to the declaration level
        new LexicalScopesBuilder().BuildFor(packageSyntax);

        // Check the semantics of the package
        var packageBuilder = CheckSemantics(packageSyntax);

        // If there are errors from the semantics phase, don't continue on
        packageBuilder.Diagnostics.ThrowIfFatalErrors();

        EntryPoint.Determine(packageBuilder);

        // If there are errors from the previous phase, don't continue on
        packageBuilder.Diagnostics.ThrowIfFatalErrors();

        return packageBuilder.Build();
    }

    private static PackageBuilder CheckSemantics(PackageSyntax<Package> packageSyntax)
    {
        DeclarationNumberAssigner.AssignIn(packageSyntax.AllEntityDeclarations);

        // Resolve symbols for the entities
        EntitySymbolBuilder.BuildFor(packageSyntax);

        // TODO handle string better
        var stringSymbol = packageSyntax.SymbolTrees
                                        .GlobalSymbols
                                        .OfType<ObjectTypeSymbol>()
                                        .SingleOrDefault(s => s.Name == "String");

        // Basic Analysis includes: Name Binding, Type Checking, Constant Folding
        BasicAnalyzer.Check(packageSyntax, stringSymbol);

        // If there are errors from the basic analysis phase, don't continue on
        packageSyntax.Diagnostics.ThrowIfFatalErrors();

#if DEBUG
        // Validate various properties of the package before continuing. Helps find bugs in the
        // analysis up to this point and avoids confusing error messages from later stages.
        new SymbolValidator(packageSyntax.SymbolTree).Validate(packageSyntax.AllEntityDeclarations);
        new TypeFulfillmentValidator().Validate(packageSyntax.AllEntityDeclarations);
        new TypeKnownValidator().Validate(packageSyntax.AllEntityDeclarations);
        new ExpressionSemanticsValidator().Validate(packageSyntax.AllEntityDeclarations);
#endif

        var packageBuilder = new ASTBuilder().BuildPackage(packageSyntax);

        // From this point forward, analysis focuses on executable declarations (i.e. invocables and field initializers)
        var executableDeclarations = packageBuilder.AllDeclarations.OfType<IExecutableDeclaration>().ToFixedSet();

        ShadowChecker.Check(executableDeclarations, packageBuilder.Diagnostics);

        DataFlowAnalysis.Check(DefiniteAssignmentAnalyzer.Instance, executableDeclarations, packageBuilder.SymbolTree, packageBuilder.Diagnostics);

        DataFlowAnalysis.Check(BindingMutabilityAnalyzer.Instance, executableDeclarations, packageBuilder.SymbolTree, packageBuilder.Diagnostics);

        DataFlowAnalysis.Check(UseOfMovedValueAnalyzer.Instance, executableDeclarations, packageBuilder.SymbolTree, packageBuilder.Diagnostics);

        // TODO use DataFlowAnalysis to check for unused variables and report use of variables starting with `_`

        // Compute variable liveness needed by reachability analyzer
        DataFlowAnalysis.Check(LivenessAnalyzer.Instance, executableDeclarations, packageBuilder.SymbolTree, packageBuilder.Diagnostics);

        // TODO remove live variables if SaveLivenessAnalysis is false

        return packageBuilder;
    }
}
