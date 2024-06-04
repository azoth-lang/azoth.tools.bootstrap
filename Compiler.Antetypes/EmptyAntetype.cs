using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

public abstract class EmptyAntetype : NonGenericNominalAntetype
{
    public SpecialTypeName Name { get; }

    protected EmptyAntetype(SpecialTypeName name)
    {
        Name = name;
    }
}
