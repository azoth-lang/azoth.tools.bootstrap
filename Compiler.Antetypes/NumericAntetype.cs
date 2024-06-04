using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

public abstract class NumericAntetype : SimpleAntetype
{
    private protected NumericAntetype(SpecialTypeName name)
        : base(name) { }
}
