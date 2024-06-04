using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Compiler.Antetypes;

public sealed class BigIntegerAntetype : IntegerAntetype
{
    internal static readonly BigIntegerAntetype Int = new(SpecialTypeName.Int, true);
    internal static readonly BigIntegerAntetype UInt = new(SpecialTypeName.UInt, false);

    private BigIntegerAntetype(SpecialTypeName name, bool isSigned)
        : base(name, isSigned)
    {
    }
}
