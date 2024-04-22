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

    private partial To.Package Transform(From.Package from)
    {
        var compilationUnits = Transform(from.CompilationUnits, packageSymbol, context.SymbolTree);
        var testingCompilationUnits = Transform(from.TestingCompilationUnits, packageSymbol, context.TestingSymbolTree);
        return Create(from, compilationUnits, testingCompilationUnits);
    }

    private partial To.CompilationUnit Transform(
        From.CompilationUnit from, PackageSymbol containingNamespace, ISymbolTreeBuilder treeBuilder)
    {
        var implicitNamespaceSymbol = BuildNamespaceSymbol(containingNamespace, from.ImplicitNamespaceName, treeBuilder);
        return Create(from, implicitNamespaceSymbol, treeBuilder);
    }

    private partial To.NamespaceDeclaration Transform(
        From.NamespaceDeclaration from,
        NamespaceSymbol containingNamespace,
        ISymbolTreeBuilder treeBuilder)
    {
        if (from.IsGlobalQualified)
            containingNamespace = packageSymbol;
        var namespaceSymbol = BuildNamespaceSymbol(containingNamespace, from.DeclaredNames, treeBuilder);
        // TODO remove properties on Syntax nodes
        from.Syntax.ContainingNamespaceSymbol = containingNamespace;
        from.Syntax.Symbol.Fulfill(namespaceSymbol);
        return Create(from, containingNamespace, namespaceSymbol, treeBuilder);
    }

    // TODO this should be more automatic
    private partial To.FunctionDeclaration Transform(
        From.FunctionDeclaration from,
        NamespaceSymbol containingNamespace)
    {
        // TODO remove properties on Syntax nodes
        from.Syntax.ContainingNamespaceSymbol = containingNamespace;
        return Create(from, containingNamespace);
    }

    private partial To.TypeDeclaration Transform(
        From.TypeDeclaration from,
        NamespaceSymbol? containingNamespace)
    {
        // TODO remove properties on Syntax nodes
        from.Syntax.ContainingNamespaceSymbol = containingNamespace!;
        return Create(from, containingNamespace, childContainingSymbol: null);
    }

    private static NamespaceSymbol BuildNamespaceSymbol(
        NamespaceSymbol containingSymbol,
        NamespaceName namespaces,
        ISymbolTreeBuilder treeBuilder)
    {
        foreach (var nsName in namespaces.Segments)
        {
            var nsSymbol = treeBuilder.GetChildrenOf(containingSymbol)
                                      .OfType<LocalNamespaceSymbol>()
                                      .SingleOrDefault(c => c.Name == nsName);
            if (nsSymbol is null)
            {
                nsSymbol = new LocalNamespaceSymbol(containingSymbol, nsName);
                treeBuilder.Add(nsSymbol);
            }

            containingSymbol = nsSymbol;
        }

        return containingSymbol;
    }
}
