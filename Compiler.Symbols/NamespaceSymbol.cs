using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

/// <summary>
/// A symbol for a namespace, whether global or local.
/// </summary>
[Closed(
    typeof(LocalNamespaceSymbol),
    typeof(PackageFacetSymbol))]
// TODO it is odd to have namespace symbols if packages don't really have namespaces declared in them
public abstract class NamespaceSymbol : Symbol
{
    public override PackageSymbol Package => Facet.Package;
    public abstract override PackageFacetSymbol Facet { get; }
    public abstract NamespaceName NamespaceName { get; }
    public abstract override IdentifierName? Name { get; }
}
