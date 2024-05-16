using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticPackageFacetSymbolNode : SemanticChildSymbolNode, IPackageFacetDeclarationNode
{
    public IdentifierName? PackageAliasOrName => Package.AliasOrName;
    public IdentifierName PackageName => Package.Name;
    public override PackageSymbol Symbol => Package.Symbol;
    public INamespaceDeclarationNode GlobalNamespace { get; }

    public SemanticPackageFacetSymbolNode(INamespaceDeclarationNode globalNamespace)
    {
        GlobalNamespace = Child.Attach(this, globalNamespace);
    }

    internal override IPackageFacetDeclarationNode InheritedFacet(IChildDeclarationNode caller, IChildDeclarationNode child)
        => this;
}
