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
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Entities;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Namespaces;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Validation;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables.BindingMutability;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables.DefiniteAssignment;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables.Moves;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables.Shadowing;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics
{
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
            var packageAbstractSyntax = CheckSemantics(packageSyntax);

            // If there are errors from the semantics phase, don't continue on
            packageAbstractSyntax.Diagnostics.ThrowIfFatalErrors();

            // Convert to IR
            //var irFactory = new IRFactory();
            //var packageIR = irFactory.CreatePackage(packageAbstractSyntax, packageAbstractSyntax.Diagnostics);

            // If there are errors from the previous phase, don't continue on
            packageAbstractSyntax.Diagnostics.ThrowIfFatalErrors();

            // TODO determine entry point

            return packageAbstractSyntax.Build();

            // Old IL Build
            //var declarationsIL = BuildIL(packageAbstractSyntax);

            //var entryPointIL = DetermineEntryPoint(declarationsIL, packageAbstractSyntax.Diagnostics);

            //var references = packageSyntax.ReferencedPackages.ToFixedSet();
            //return new PackageIL(packageAbstractSyntax.SymbolTree, packageAbstractSyntax.Diagnostics.Build(), references, declarationsIL, entryPointIL);
        }

        private static PackageBuilder CheckSemantics(PackageSyntax<Package> packageSyntax)
        {
            DeclarationNumberAssigner.AssignIn(packageSyntax.AllEntityDeclarations);

            // Resolve symbols for the entities
            EntitySymbolBuilder.BuildFor(packageSyntax);

            var stringSymbol = packageSyntax.SymbolTrees
                                            .GlobalSymbols
                                            .OfType<ObjectTypeSymbol>()
                                            .SingleOrDefault(s => s.Name == "string");

            // Basic Analysis includes: Name Binding, Type Checking, Constant Folding
            BasicAnalyzer.Check(packageSyntax, stringSymbol);

            // If there are errors from the basic analysis phase, don't continue on
            packageSyntax.Diagnostics.ThrowIfFatalErrors();

#if DEBUG
            new SymbolValidator(packageSyntax.SymbolTree).Validate(packageSyntax.AllEntityDeclarations);
            new TypeFulfillmentValidator().Validate(packageSyntax.AllEntityDeclarations);
            new TypeKnownValidator().Validate(packageSyntax.AllEntityDeclarations);
            new ExpressionSemanticsValidator().Validate(packageSyntax.AllEntityDeclarations);
#endif

            var package = new ASTBuilder().BuildPackage(packageSyntax);

            // From this point forward, analysis focuses on executable declarations (i.e. invocables and field initializers)
            var executableDeclarations = package.AllDeclarations.OfType<IExecutableDeclaration>().ToFixedSet();

            ShadowChecker.Check(executableDeclarations, package.Diagnostics);

            DataFlowAnalysis.Check(DefiniteAssignmentAnalyzer.Instance, executableDeclarations, package.SymbolTree, package.Diagnostics);

            DataFlowAnalysis.Check(BindingMutabilityAnalyzer.Instance, executableDeclarations, package.SymbolTree, package.Diagnostics);

            DataFlowAnalysis.Check(UseOfMovedValueAnalyzer.Instance, executableDeclarations, package.SymbolTree, package.Diagnostics);

            // TODO use DataFlowAnalysis to check for unused variables and report use of variables starting with `_`

            // Compute variable liveness needed by reachability analyzer
            DataFlowAnalysis.Check(LivenessAnalyzer.Instance, executableDeclarations, package.SymbolTree, package.Diagnostics);

            // TODO remove live variables if SaveLivenessAnalysis is false

            return package;
        }

        //private static FixedList<DeclarationIL> BuildIL(Package package)
        //{
        //    var ilFactory = new ILFactory();
        //    var declarationBuilder = new DeclarationBuilder(ilFactory);
        //    declarationBuilder.Build(package.AllDeclarations, package.SymbolTree);
        //    return declarationBuilder.AllDeclarations.ToFixedList();
        //}

        //private static FunctionIL? DetermineEntryPoint(
        //    FixedList<DeclarationIL> declarations,
        //    Diagnostics diagnostics)
        //{
        //    var mainFunctions = declarations.OfType<FunctionIL>()
        //        .Where(f => f.Symbol.Name == "main")
        //        .ToList();

        //    // TODO warn on and remove main functions that don't have correct parameters or types
        //    _ = diagnostics;
        //    // TODO compiler error on multiple main functions

        //    return mainFunctions.SingleOrDefault();
        //}
    }
}
