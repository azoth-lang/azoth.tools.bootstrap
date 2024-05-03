using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal class ReferencedPackageSymbolNode : ReferencedSymbolNode, IPackageSymbolNode
{
    public override PackageSymbol Symbol { get; }
    public IdentifierName AliasOrName { get; }
    public IdentifierName Name => Symbol.Name;

    public IFacetSymbolNode MainFacet { get; }
    public IFacetSymbolNode TestingFacet { get; }

    public ReferencedPackageSymbolNode(IPackageReferenceNode node)
    {
        var symbols = node.PackageSymbols;
        Symbol = symbols.PackageSymbol;
        AliasOrName = node.AliasOrName;
        MainFacet = Child.Attach(this, new ReferencedFacetSymbolNode(symbols.SymbolTree));
        TestingFacet = Child.Attach(this, new ReferencedFacetSymbolNode(symbols.TestingSymbolTree));
    }

    internal override IPackageSymbolNode InheritedPackage(IChildSymbolNode caller, IChildSymbolNode child)
        => this;
}
