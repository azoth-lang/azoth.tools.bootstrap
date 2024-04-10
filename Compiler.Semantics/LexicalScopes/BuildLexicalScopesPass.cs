using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.IST;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;
using Void = Azoth.Tools.Bootstrap.Framework.Void;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

public partial class BuildLexicalScopesPass
{
    private BuildLexicalScopesPass(Void context) { }

    private partial Scoped.Package Transform(Concrete.Package from)
    {
        var packagesScope = BuildPackagesScope(from);

        //var primitiveEntitySymbols = GetPrimitiveEntitySymbols().ToFixedList();
        //var declarationSymbols = GetAllNonMemberDeclarationSymbols(primitiveEntitySymbols, package.SymbolTree,
        //    package.EntityDeclarations, package.ReferencedPackages.Select(p => p.SymbolTree));
        //var compilationUnits = Transform(from.CompilationUnits, packagesScope, declarationSymbols);

        //var testingDeclarationSymbols = GetAllNonMemberDeclarationSymbols(primitiveEntitySymbols,
        //    package.TestingSymbolTree, package.AllEntityDeclarations,
        //    package.ReferencedPackages.Select(p => p.TestingSymbolTree));
        //var testingCompilationUnits = Transform(from.TestingCompilationUnits, packagesScope, testingDeclarationSymbols);

        //return Transform(from, compilationUnits, testingCompilationUnits);

        throw new NotImplementedException();
    }

    private partial (NestedScope, FixedDictionary<NamespaceName, Namespace> namespaces, LexicalScope) Enter(
        IFixedList<Concrete.CompilationUnit> from,
        PackagesScope packagesScope,
        IFixedList<NonMemberSymbol> declarationSymbols)
    {
        var namespaces = BuildNamespaces(declarationSymbols);
        var globalScope = BuildGlobalScope(packagesScope, namespaces[NamespaceName.Global]);
        return (globalScope, namespaces, globalScope);
    }

    private partial Scoped.Declaration Transform(Concrete.Declaration from, LexicalScope containingScope)
        => throw new NotImplementedException();

    private static IFixedList<NonMemberSymbol> GetAllNonMemberDeclarationSymbols(
        IEnumerable<NonMemberSymbol> primitiveEntitySymbols,
        ISymbolTree packageSymbolTree,
        IEnumerable<IEntityDeclarationSyntax> packageEntityDeclarations,
        IEnumerable<FixedSymbolTree> referencedSymbolTrees)
    {
        // Namespaces in the package need to be created even if they are empty
        var packageNamespaces = packageSymbolTree.Symbols.OfType<LocalNamespaceSymbol>()
                                                 .Select(NonMemberSymbol.ForPackageNamespace);

        var packageNonMemberEntitySymbols = packageEntityDeclarations.OfType<INonMemberEntityDeclarationSyntax>()
                                                                     .Select(NonMemberSymbol.For);

        // TODO it might be better to go to the declarations and get their symbols (once that is implemented)
        var referencedSymbols = referencedSymbolTrees.SelectMany(t => t.Symbols).Concat(Intrinsic.SymbolTree.Symbols)
                                                     .Where(s => s.ContainingSymbol is NamespaceOrPackageSymbol)
                                                     .Select(NonMemberSymbol.ForExternalSymbol);
        return primitiveEntitySymbols.Concat(packageNamespaces).Concat(packageNonMemberEntitySymbols)
                                     .Concat(referencedSymbols).ToFixedList();
    }


    private static IEnumerable<NonMemberSymbol> GetPrimitiveEntitySymbols()
        => Primitive.SymbolTree.Symbols.Where(s => s.ContainingSymbol is null)
                    .Select(NonMemberSymbol.ForExternalSymbol);

    private static FixedDictionary<NamespaceName, Namespace> BuildNamespaces(
        IFixedList<NonMemberSymbol> declarationSymbols)
    {
        // Use RequiredNamespace so that namespaces in the package will be created even if they are empty
        var namespaces = declarationSymbols.SelectMany(s => s.RequiredNamespace.NamespaceNames()).Distinct();
        var nsSymbols = new List<Namespace>();
        foreach (var ns in namespaces)
        {
            var symbols = declarationSymbols.Where(s => s.ContainingNamespace == ns).ToList();
            var nestedSymbols = declarationSymbols.Where(s => s.ContainingNamespace.IsNestedIn(ns)).ToList();

            nsSymbols.Add(new Namespace(ns, ToDictionary(symbols), ToDictionary(nestedSymbols),
                ToDictionary(symbols.Where(s => s.InCurrentPackage)),
                ToDictionary(nestedSymbols.Where(s => s.InCurrentPackage))));
        }

        return nsSymbols.ToFixedDictionary(ns => ns.Name);
    }

    private static PackagesScope BuildPackagesScope(Concrete.Package package)
    {
        var syntax = (PackageSyntax<Package>)package.Syntax;
        var packageAliases = syntax.References.ToDictionary(r => r.AliasOrName, r => r.Package.Symbol).ToFixedDictionary();
        return new PackagesScope(package.Symbol, packageAliases);
    }

    private static NestedScope BuildGlobalScope(PackagesScope packagesScope, Namespace globalNamespace)
    {
        var allPackagesGlobalScope
            = NestedScope.CreateGlobal(packagesScope, globalNamespace.Symbols, globalNamespace.NestedSymbols);

        return allPackagesGlobalScope;
    }

    private static FixedDictionary<TypeName, IFixedSet<IPromise<Symbol>>> ToDictionary(
        IEnumerable<NonMemberSymbol> symbols)
        => symbols.GroupBy(s => s.Name, s => s.Symbol).ToFixedDictionary(e => e.Key, e => e.ToFixedSet());
}
