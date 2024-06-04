using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Compiler.Antetypes.ConstValue;

public sealed class IntegerConstValueAntetype : ConstValueAntetype
{
    public BigInteger Value { get; }

    public IntegerConstValueAntetype(BigInteger value)
        : base(SpecialTypeName.ConstInt)
    {
        Value = value;
    }
}
