using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(
    typeof(PrimitiveTypeSymbol),
    typeof(ObjectTypeSymbol),
    typeof(GenericParameterTypeSymbol))]
public abstract class TypeSymbol : Symbol
{
    public override Name Name { get; }

    protected TypeSymbol(Symbol? containingSymbol, Name name)
        : base(containingSymbol, name)
    {
        Name = name;
    }
}
