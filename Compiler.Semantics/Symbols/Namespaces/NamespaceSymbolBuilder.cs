using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Contexts;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using From = Azoth.Tools.Bootstrap.Compiler.IST.Concrete;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithNamespaceSymbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Namespaces;

internal sealed partial class NamespaceSymbolBuilder
{
    private readonly SymbolBuilderContext context;
    private readonly PackageSymbol packageSymbol;

    private NamespaceSymbolBuilder(SymbolBuilderContext context)
    {
        this.context = context;
        packageSymbol = context.SymbolTree.Package.Assigned();
    }

    private partial SymbolBuilderContext EndRun(To.Package package) => context;

    private partial To.Package Transform(From.Package value)
    {
        var compilationUnits = Transform(value.CompilationUnits, packageSymbol, context.SymbolTree);
        var testingCompilationUnits = Transform(value.TestingCompilationUnits, packageSymbol, context.TestingSymbolTree);
        return Create(value, compilationUnits, testingCompilationUnits);
    }

    private partial To.CompilationUnit Transform(
        From.CompilationUnit from, PackageSymbol containingSymbol, ISymbolTreeBuilder treeBuilder)
    {
        var sym = BuildNamespaceSymbol(containingSymbol, from.ImplicitNamespaceName, treeBuilder);
        return Create(from, sym, treeBuilder);
    }

    private partial To.NamespaceDeclaration Transform(
        From.NamespaceDeclaration from,
        NamespaceOrPackageSymbol containingSymbol,
        ISymbolTreeBuilder treeBuilder)
    {
        if (from.IsGlobalQualified)
            containingSymbol = packageSymbol;
        var sym = BuildNamespaceSymbol(containingSymbol, from.DeclaredNames, treeBuilder);
        return Create(from, containingSymbol, sym, treeBuilder);
    }

    private partial To.FunctionDeclaration Transform(
        From.FunctionDeclaration from,
        NamespaceOrPackageSymbol containingSymbol)
        => Create(from, containingSymbol);

    private partial To.TypeDeclaration Transform(
        From.TypeDeclaration from,
        NamespaceOrPackageSymbol? containingSymbol)
        => Create(from, containingSymbol, childContainingSymbol: null);

    private static NamespaceOrPackageSymbol BuildNamespaceSymbol(
        NamespaceOrPackageSymbol containingSymbol,
        NamespaceName namespaces,
        ISymbolTreeBuilder treeBuilder)
    {
        foreach (var nsName in namespaces.Segments)
        {
            var nsSymbol = treeBuilder.GetChildrenOf(containingSymbol)
                                      .OfType<NamespaceSymbol>()
                                      .SingleOrDefault(c => c.Name == nsName);
            if (nsSymbol is null)
            {
                nsSymbol = new NamespaceSymbol(containingSymbol, nsName);
                treeBuilder.Add(nsSymbol);
            }

            containingSymbol = nsSymbol;
        }

        return containingSymbol;
    }
}
