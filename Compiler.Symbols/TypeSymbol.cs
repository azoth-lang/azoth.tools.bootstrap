using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(
    typeof(EmptyTypeSymbol),
    typeof(PrimitiveTypeSymbol),
    typeof(ObjectTypeSymbol),
    typeof(GenericParameterTypeSymbol))]
public abstract class TypeSymbol : Symbol
{
    public override TypeName Name { get; }

    public bool IsGlobal => ContainingSymbol == Package;

    protected TypeSymbol(TypeName name)
        : base(name)
    {
        Name = name;
    }
}
