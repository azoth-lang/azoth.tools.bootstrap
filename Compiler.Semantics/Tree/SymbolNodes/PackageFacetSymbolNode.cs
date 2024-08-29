using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal sealed class PackageFacetSymbolNode : ChildSymbolNode, IPackageFacetSymbolNode
{
    public IdentifierName? PackageAliasOrName => Package.AliasOrName;
    public IdentifierName PackageName => Package.Name;
    public PackageSymbol PackageSymbol => Package.Symbol;
    public override PackageSymbol Symbol => PackageSymbol;
    public FixedSymbolTree SymbolTree { get; }

    public INamespaceDeclarationNode GlobalNamespace { get; }

    public PackageFacetSymbolNode(FixedSymbolTree symbolTree)
    {
        SymbolTree = symbolTree;
        GlobalNamespace = Child.Attach(this, new NamespaceSymbolNode(symbolTree.Package));
    }

    internal override ISymbolTree Inherited_SymbolTree(IChildNode child, IChildNode descendant)
        => SymbolTree;

    internal override IPackageFacetDeclarationNode Inherited_Facet(IChildNode child, IChildNode descendant)
        => this;
}
