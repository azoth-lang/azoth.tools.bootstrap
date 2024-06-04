using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Compiler.Antetypes;

public sealed class FixedSizeIntegerAntetype : IntegerAntetype
{
    internal static readonly FixedSizeIntegerAntetype Int8 = new(SpecialTypeName.Int8, -8);
    internal static readonly FixedSizeIntegerAntetype Byte = new(SpecialTypeName.Byte, 8);
    internal static readonly FixedSizeIntegerAntetype Int16 = new(SpecialTypeName.Int16, -16);
    internal static readonly FixedSizeIntegerAntetype UInt16 = new(SpecialTypeName.UInt16, 16);
    internal static readonly FixedSizeIntegerAntetype Int32 = new(SpecialTypeName.Int32, -32);
    internal static readonly FixedSizeIntegerAntetype UInt32 = new(SpecialTypeName.UInt32, 32);
    internal static readonly FixedSizeIntegerAntetype Int64 = new(SpecialTypeName.Int64, -64);
    internal static readonly FixedSizeIntegerAntetype UInt64 = new(SpecialTypeName.UInt64, 64);

    public int Bits { get; }
    public BigInteger MaxValue;
    public BigInteger MinValue;

    private FixedSizeIntegerAntetype(SpecialTypeName name, int bits)
        : base(name, bits < 0)
    {
        Bits = Math.Abs(bits);
        if (IsSigned)
        {
            var powerOf2 = BigInteger.Pow(2, Bits - 1);
            MinValue = -powerOf2;
            MaxValue = powerOf2 - 1;
        }
        else
        {
            MinValue = 0;
            MaxValue = BigInteger.Pow(2, Bits);
        }
    }
}
