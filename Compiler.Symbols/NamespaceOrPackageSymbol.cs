using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(
    typeof(NamespaceSymbol),
    typeof(PackageSymbol))]
public abstract class NamespaceOrPackageSymbol : Symbol
{
    public NamespaceName NamespaceName { get; }
    public override SimpleName Name { get; }

    protected NamespaceOrPackageSymbol(NamespaceOrPackageSymbol? containingSymbol, SimpleName name)
        : base(containingSymbol, name)
    {
        NamespaceName = containingSymbol is null
            ? NamespaceName.Global
            : containingSymbol.NamespaceName.Qualify(name);
        Name = name;
    }
}
