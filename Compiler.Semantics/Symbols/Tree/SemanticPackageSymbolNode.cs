using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticPackageSymbolNode : SemanticSymbolNode, IPackageDeclarationNode
{
    private IPackageNode Node { get; }
    public IdentifierName? AliasOrName => null;
    public override PackageSymbol Symbol => Node.Symbol;
    public IdentifierName Name => Symbol.Name;
    public IPackageFacetDeclarationNode MainFacet { get; }
    public IPackageFacetDeclarationNode TestingFacet { get; }

    public SemanticPackageSymbolNode(
        IPackageNode node,
        IPackageFacetDeclarationNode mainFacet,
        IPackageFacetDeclarationNode testingFacet)
    {
        Node = node;
        MainFacet = Child.Attach(this, mainFacet);
        TestingFacet = Child.Attach(this, testingFacet);
    }

    internal override IPackageDeclarationNode InheritedPackage(IChildDeclarationNode caller, IChildDeclarationNode child)
        => this;
}
