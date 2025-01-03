using System.Diagnostics;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

public sealed class FixedSizeIntegerTypeConstructor : IntegerTypeConstructor
{
    internal new static readonly FixedSizeIntegerTypeConstructor Int8 = new(BuiltInTypeName.Int8, 8, true);
    internal new static readonly FixedSizeIntegerTypeConstructor Byte = new(BuiltInTypeName.Byte, 8, false);
    internal new static readonly FixedSizeIntegerTypeConstructor Int16 = new(BuiltInTypeName.Int16, 16, true);
    internal new static readonly FixedSizeIntegerTypeConstructor UInt16 = new(BuiltInTypeName.UInt16, 16, false);
    internal new static readonly FixedSizeIntegerTypeConstructor Int32 = new(BuiltInTypeName.Int32, 32, true);
    internal new static readonly FixedSizeIntegerTypeConstructor UInt32 = new(BuiltInTypeName.UInt32, 32, false);
    internal new static readonly FixedSizeIntegerTypeConstructor Int64 = new(BuiltInTypeName.Int64, 64, true);
    internal new static readonly FixedSizeIntegerTypeConstructor UInt64 = new(BuiltInTypeName.UInt64, 64, false);

    public int Bits { [DebuggerStepThrough] get; }
    public BigInteger MaxValue;
    public BigInteger MinValue;

    private FixedSizeIntegerTypeConstructor(BuiltInTypeName name, int bits, bool isSigned)
        : base(name, isSigned)
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

    /// <remarks>If the current type is already signed then this doesn't change anything. If the
    /// current type is unsigned, then this returns the next larger integer type.</remarks>
    public override IntegerTypeConstructor WithSign()
    {
        if (IsSigned) return this;
        if (ReferenceEquals(this, Byte)) return Int16;
        if (ReferenceEquals(this, UInt16)) return Int32;
        if (ReferenceEquals(this, UInt32)) return Int64;
        if (ReferenceEquals(this, UInt64)) return Int;
        throw new UnreachableException("All values should be covered.");
    }
}
