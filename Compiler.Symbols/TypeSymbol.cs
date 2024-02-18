using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(
    typeof(PrimitiveTypeSymbol),
    typeof(ObjectTypeSymbol),
    typeof(GenericParameterTypeSymbol))]
public abstract class TypeSymbol : Symbol
{
    public override TypeName Name { get; }

    protected TypeSymbol(TypeName name)
        : base(name)
    {
        Name = name;
    }
}
