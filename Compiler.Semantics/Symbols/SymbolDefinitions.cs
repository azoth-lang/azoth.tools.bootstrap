using System;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal static class SymbolDefinitions
{
    public static PackageSymbol Package(IPackageNode package) => new PackageSymbol(package.Name);

    public static NamespaceSymbol NamespaceDeclaration(INamespaceDeclarationNode node)
    {
        var containingSymbol = node.ContainingNamespace;
        foreach (var nsName in node.DeclaredNames.Segments)
        {
            throw new NotImplementedException();
            //var nsSymbol = treeBuilder.GetChildrenOf(containingSymbol).OfType<NamespaceSymbol>()
            //                          .SingleOrDefault(c => c.Name == nsName);
            //if (nsSymbol is null)
            //{
            //    nsSymbol = new LocalNamespaceSymbol(containingSymbol, nsName);
            //    treeBuilder.Add(nsSymbol);
            //}

            //containingSymbol = nsSymbol;
        }

        return containingSymbol;
    }
}
