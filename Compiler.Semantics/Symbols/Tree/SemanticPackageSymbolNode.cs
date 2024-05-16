using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticPackageSymbolNode : SemanticNode, IPackageSymbolNode
{
    private IPackageNode Node { get; }
    public override ISyntax? Syntax => Node.Syntax;
    public IdentifierName? AliasOrName => null;
    public PackageSymbol Symbol => Node.Symbol;
    public IdentifierName Name => Symbol.Name;
    public IPackageFacetDeclarationNode MainFacet => Node.MainFacet.SymbolNode;
    public IPackageFacetDeclarationNode TestingFacet => Node.TestingFacet.SymbolNode;

    public SemanticPackageSymbolNode(IPackageNode node)
    {
        Node = node;
    }

    internal override IPackageDeclarationNode InheritedPackage(IChildNode caller, IChildNode child)
        => this;
}
