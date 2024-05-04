using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticPackageFacetSymbolNode : SemanticChildSymbolNode, IPackageFacetSymbolNode
{
    public IdentifierName? PackageAliasOrName => Package.AliasOrName;
    public IdentifierName PackageName => Package.Name;
    public override PackageSymbol Symbol => Package.Symbol;
    public INamespaceSymbolNode GlobalNamespace { get; }

    public SemanticPackageFacetSymbolNode(INamespaceSymbolNode globalNamespace)
    {
        GlobalNamespace = Child.Attach(this, globalNamespace);
    }

    internal override IPackageFacetSymbolNode InheritedFacet(IChildSymbolNode caller, IChildSymbolNode child)
        => this;
}
