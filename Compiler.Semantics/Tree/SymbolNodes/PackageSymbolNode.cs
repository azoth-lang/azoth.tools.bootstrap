using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal class PackageSymbolNode : SemanticNode, IPackageSymbolNode
{
    public override ISyntax? Syntax => null;
    public PackageSymbol Symbol { get; }
    public IdentifierName AliasOrName { get; }
    public IdentifierName Name => Symbol.Name;

    public IPackageFacetDeclarationNode MainFacet { get; }
    public IPackageFacetDeclarationNode TestingFacet { get; }

    public PackageSymbolNode(IPackageReferenceNode node)
    {
        var symbols = node.PackageSymbols;
        Symbol = symbols.PackageSymbol;
        AliasOrName = node.AliasOrName;
        MainFacet = Child.Attach(this, new PackageFacetSymbolNode(symbols.SymbolTree));
        TestingFacet = Child.Attach(this, new PackageFacetSymbolNode(symbols.TestingSymbolTree));
    }

    internal override IPackageDeclarationNode InheritedPackage(IChildNode child, IChildNode descendant)
        => this;
}
