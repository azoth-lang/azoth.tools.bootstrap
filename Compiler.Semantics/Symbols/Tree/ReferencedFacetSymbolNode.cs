using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class ReferencedFacetSymbolNode : ReferencedChildSymbolNode, IFacetSymbolNode
{
    public IdentifierName? PackageAliasOrName => Package.AliasOrName;
    public IdentifierName PackageName => Package.Name;
    public override PackageSymbol Symbol => Package.Symbol;

    private readonly FixedSymbolTree symbolTree;

    public INamespaceSymbolNode GlobalNamespace { get; }

    public ReferencedFacetSymbolNode(FixedSymbolTree symbolTree)
    {
        this.symbolTree = symbolTree;
        GlobalNamespace = Child.Attach(this, new ReferencedNamespaceSymbolNode(symbolTree.Package));
    }

    internal override ISymbolTree InheritedSymbolTree(IChildSymbolNode caller, IChildSymbolNode child)
        => symbolTree;

    internal override IFacetSymbolNode InheritedFacet(IChildSymbolNode caller, IChildSymbolNode child)
        => this;
}
