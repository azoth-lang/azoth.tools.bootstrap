using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

/// <summary>
/// Type constructor for arbitrary-precision integers.
/// </summary>
/// <remarks>While these are just the basic integer types in Azoth, a name was needed to distinguish
/// this class from the base class for all integer types. While not ideal,
/// <see cref="BigIntegerTypeConstructor"/> is the best short name available.</remarks>
public sealed class BigIntegerTypeConstructor : IntegerTypeConstructor
{
    internal static readonly BigIntegerTypeConstructor Int = new(SpecialTypeName.Int, true);
    internal static readonly BigIntegerTypeConstructor UInt = new(SpecialTypeName.UInt, false);

    private BigIntegerTypeConstructor(SpecialTypeName name, bool isSigned)
        : base(name, isSigned)
    {
    }

    public override IntegerTypeConstructor WithSign() => TypeConstructor.Int;
}
