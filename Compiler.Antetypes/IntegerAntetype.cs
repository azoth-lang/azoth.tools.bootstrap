using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Compiler.Antetypes;

public abstract class IntegerAntetype : NumericAntetype
{
    public bool IsSigned { get; }

    private protected IntegerAntetype(SpecialTypeName name, bool isSigned)
        : base(name)
    {
        IsSigned = isSigned;
    }
}
