using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Walkers;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Namespaces;

public class NamespaceSymbolBuilder : SyntaxWalker<NamespaceOrPackageSymbol>
{
    private readonly ISymbolTreeBuilder treeBuilder;
    private readonly PackageSymbol packageSymbol;

    private NamespaceSymbolBuilder(ISymbolTreeBuilder treeBuilder, PackageSymbol packageSymbol)
    {
        this.treeBuilder = treeBuilder;
        this.packageSymbol = packageSymbol;
    }

    public static void BuildNamespaceSymbols(PackageSyntax<Package> package)
    {
        BuildNamespaceSymbols(package, package.SymbolTree, package.CompilationUnits);
        BuildNamespaceSymbols(package, package.TestingSymbolTree, package.TestingCompilationUnits);
    }

    private static void BuildNamespaceSymbols(
        PackageSyntax<Package> package,
        ISymbolTreeBuilder treeBuilder,
        IFixedSet<ICompilationUnitSyntax> compilationUnits)
    {
        var builder = new NamespaceSymbolBuilder(treeBuilder, package.Symbol);
        foreach (var compilationUnit in compilationUnits)
            builder.Walk(compilationUnit, package.Symbol);
    }

    protected override void WalkNonNull(ISyntax syntax, NamespaceOrPackageSymbol containingSymbol)
    {
        switch (syntax)
        {
            case ICompilationUnitSyntax syn:
            {
                var sym = BuildNamespaceSymbol(containingSymbol, syn.ImplicitNamespaceName);
                WalkChildren(syn, sym);
            }
            break;
            case INamespaceDeclarationSyntax syn:
            {
                syn.ContainingNamespaceSymbol = containingSymbol;
                var sym = BuildNamespaceSymbol(syn.IsGlobalQualified
                    ? packageSymbol : containingSymbol, syn.DeclaredNames);
                syn.Symbol.Fulfill(sym);
                WalkChildren(syn, sym);
            }
            break;
            case INonMemberEntityDeclarationSyntax syn:
                syn.ContainingNamespaceSymbol = containingSymbol;
                break;
            default:
                // do nothing
                return;
        }
    }

    private NamespaceOrPackageSymbol BuildNamespaceSymbol(
        NamespaceOrPackageSymbol containingSymbol,
        NamespaceName namespaces)
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
