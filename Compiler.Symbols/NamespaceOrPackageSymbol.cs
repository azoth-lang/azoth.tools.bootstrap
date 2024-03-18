using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(
    typeof(NamespaceSymbol),
    typeof(PackageSymbol))]
public abstract class NamespaceOrPackageSymbol : Symbol
{
    public override PackageSymbol Package { get; }
    public NamespaceName NamespaceName { get; }
    public override IdentifierName Name { get; }

    protected NamespaceOrPackageSymbol(PackageSymbol package, NamespaceOrPackageSymbol? containingSymbol, IdentifierName name)
    {
        Package = package;
        NamespaceName = containingSymbol is null
            ? NamespaceName.Global
            : containingSymbol.NamespaceName.Qualify(name);
        Name = name;
    }
}
