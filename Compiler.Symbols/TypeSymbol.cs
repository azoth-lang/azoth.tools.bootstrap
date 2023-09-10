using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(
    typeof(PrimitiveTypeSymbol),
    typeof(ObjectTypeSymbol))]
public abstract class TypeSymbol : Symbol
{
    public new NamespaceOrPackageSymbol? ContainingSymbol { get; }
    public new TypeName Name { get; }

    protected TypeSymbol(NamespaceOrPackageSymbol? containingSymbol, TypeName name)
        : base(containingSymbol, name)
    {
        ContainingSymbol = containingSymbol;
        Name = name;
    }
}
