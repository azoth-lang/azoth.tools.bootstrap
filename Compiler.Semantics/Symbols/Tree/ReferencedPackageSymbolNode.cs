using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal class ReferencedPackageSymbolNode : ReferencedSymbolNode, IPackageSymbolNode
{
    public override PackageSymbol Symbol { get; }

    public IdentifierName AliasOrName { get; }
    public IdentifierName Name => Symbol.Name;

    private readonly ISymbolTree symbolTree;
    public INamespaceSymbolNode GlobalNamespace { get; }
    private readonly ISymbolTree testingSymbolTree;
    public INamespaceSymbolNode TestingGlobalNamespace { get; }

    public ReferencedPackageSymbolNode(IPackageReferenceNode node)
    {
        var symbols = node.PackageSymbols;
        Symbol = symbols.PackageSymbol;
        AliasOrName = node.AliasOrName;
        symbolTree = symbols.SymbolTree;
        GlobalNamespace = new ReferencedNamespaceSymbolNode(symbols.PackageSymbol);
        testingSymbolTree = symbols.TestingSymbolTree;
        TestingGlobalNamespace = new ReferencedNamespaceSymbolNode(symbols.PackageSymbol);
    }

    internal override IPackageSymbolNode InheritedPackage(IChildSymbolNode caller, IChildSymbolNode child)
        => this;

    internal override ISymbolTree InheritedSymbolTree(IChildSymbolNode caller, IChildSymbolNode child)
    {
        if (caller == GlobalNamespace)
            return symbolTree;
        if (caller == TestingGlobalNamespace)
            return testingSymbolTree;
        return base.InheritedSymbolTree(caller, child);
    }
}
