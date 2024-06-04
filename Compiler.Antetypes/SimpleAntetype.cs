using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

public abstract class SimpleAntetype : INonVoidAntetype, IDeclaredAntetype
{
    public SpecialTypeName Name { get; }

    private protected SimpleAntetype(SpecialTypeName name)
    {
        Name = name;
    }

    public IAntetype With(IEnumerable<IAntetype> typeArguments)
    {
        if (typeArguments.Any())
            throw new ArgumentException("Simple type cannot have type arguments", nameof(typeArguments));
        return this;
    }
}
