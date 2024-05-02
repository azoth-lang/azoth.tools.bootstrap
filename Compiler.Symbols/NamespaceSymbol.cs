using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

/// <summary>
/// A symbol for a namespace, whether global or local.
/// </summary>
[Closed(
    typeof(LocalNamespaceSymbol),
    typeof(PackageSymbol))]
public abstract class NamespaceSymbol : Symbol
{
    public override PackageSymbol Package { get; }
    public NamespaceName NamespaceName { get; }
    public override IdentifierName Name { get; }

    protected NamespaceSymbol(PackageSymbol package, NamespaceSymbol? containingSymbol, IdentifierName name)
    {
        Package = package;
        NamespaceName = containingSymbol is null
            ? NamespaceName.Global
            : containingSymbol.NamespaceName.Qualify(name);
        Name = name;
    }
}
