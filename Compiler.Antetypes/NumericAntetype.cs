using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Compiler.Antetypes;

public abstract class NumericAntetype : SimpleAntetype
{
    private protected NumericAntetype(SpecialTypeName name)
        : base(name) { }
}
