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
    internal new static readonly BigIntegerTypeConstructor Int = new(BuiltInTypeName.Int, true);
    internal new static readonly BigIntegerTypeConstructor UInt = new(BuiltInTypeName.UInt, false);

    private BigIntegerTypeConstructor(BuiltInTypeName name, bool isSigned)
        : base(name, isSigned)
    {
    }

    public override IntegerTypeConstructor WithSign() => TypeConstructor.Int;
}
