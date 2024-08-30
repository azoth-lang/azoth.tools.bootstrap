using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal class PackageSymbolNode : ChildNode, IPackageSymbolNode
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
        MainFacet = Child.Attach(this, IPackageFacetSymbolNode.Create(symbols.SymbolTree));
        TestingFacet = Child.Attach(this, IPackageFacetSymbolNode.Create(symbols.TestingSymbolTree));
    }

    internal override IPackageDeclarationNode Inherited_Package(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => this;
}
