using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

public abstract class IntegerAntetype : NumericAntetype
{
    public bool IsSigned { get; }

    private protected IntegerAntetype(SpecialTypeName name, bool isSigned)
        : base(name)
    {
        IsSigned = isSigned;
    }
}
