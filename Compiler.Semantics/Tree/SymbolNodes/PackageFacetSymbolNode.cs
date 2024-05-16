using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal sealed class PackageFacetSymbolNode : ChildSymbolNode, IPackageFacetSymbolNode
{
    public IdentifierName? PackageAliasOrName => Package.AliasOrName;
    public IdentifierName PackageName => Package.Name;
    public override PackageSymbol Symbol => Package.Symbol;

    private readonly FixedSymbolTree symbolTree;

    public INamespaceDeclarationNode GlobalNamespace { get; }

    public PackageFacetSymbolNode(FixedSymbolTree symbolTree)
    {
        this.symbolTree = symbolTree;
        GlobalNamespace = Child.Attach(this, new NamespaceSymbolNode(symbolTree.Package));
    }

    internal override ISymbolTree InheritedSymbolTree(IChildNode caller, IChildNode child)
        => symbolTree;

    internal override IPackageFacetDeclarationNode InheritedFacet(IChildNode caller, IChildNode child)
        => this;
}
