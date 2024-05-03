using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticPackageSymbolNode : SemanticSymbolNode, IPackageSymbolNode
{
    private IPackageNode Node { get; }
    public IdentifierName? AliasOrName => null;
    public override PackageSymbol Symbol => Node.Symbol;
    public IdentifierName Name => Symbol.Name;
    public IFacetSymbolNode MainFacet { get; }
    public IFacetSymbolNode TestingFacet { get; }

    public SemanticPackageSymbolNode(
        IPackageNode node,
        IFacetSymbolNode mainFacet,
        IFacetSymbolNode testingFacet)
    {
        Node = node;
        MainFacet = Child.Attach(this, mainFacet);
        TestingFacet = Child.Attach(this, testingFacet);
    }

    internal override IPackageSymbolNode InheritedPackage(IChildSymbolNode caller, IChildSymbolNode child)
        => this;
}
