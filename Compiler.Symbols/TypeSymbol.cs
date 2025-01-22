using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(
    typeof(NeverTypeSymbol),
    typeof(VoidTypeSymbol),
    typeof(BuiltInTypeSymbol),
    typeof(OrdinaryTypeSymbol),
    typeof(GenericParameterTypeSymbol),
    typeof(AssociatedTypeSymbol))]
public abstract class TypeSymbol : Symbol
{
    public override UnqualifiedName Name { get; }

    public bool IsGlobal => ContainingSymbol == Package;

    protected TypeSymbol(UnqualifiedName name)
    {
        Name = name;
    }

    public virtual BareTypeConstructor? TryGetTypeConstructor() => null;

    public virtual PlainType? TryGetPlainType() => null;
}
