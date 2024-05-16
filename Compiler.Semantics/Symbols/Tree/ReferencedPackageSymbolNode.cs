using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal class ReferencedPackageSymbolNode : ReferencedSymbolNode, IPackageDeclarationNode
{
    public override PackageSymbol Symbol { get; }
    public IdentifierName AliasOrName { get; }
    public IdentifierName Name => Symbol.Name;

    public IPackageFacetDeclarationNode MainFacet { get; }
    public IPackageFacetDeclarationNode TestingFacet { get; }

    public ReferencedPackageSymbolNode(IPackageReferenceNode node)
    {
        var symbols = node.PackageSymbols;
        Symbol = symbols.PackageSymbol;
        AliasOrName = node.AliasOrName;
        MainFacet = Child.Attach(this, new ReferencedPackageFacetSymbolNode(symbols.SymbolTree));
        TestingFacet = Child.Attach(this, new ReferencedPackageFacetSymbolNode(symbols.TestingSymbolTree));
    }

    internal override IPackageDeclarationNode InheritedPackage(IChildDeclarationNode caller, IChildDeclarationNode child)
        => this;
}
