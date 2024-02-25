using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Declared;

public sealed class BigIntegerType : IntegerType
{
    internal new static readonly BigIntegerType Int = new(SpecialTypeName.Int, true);
    internal new static readonly BigIntegerType UInt = new(SpecialTypeName.UInt, false);

    public override BareValueType<BigIntegerType> BareType { get; }

    public override ValueType<BigIntegerType> Type { get; }

    private BigIntegerType(SpecialTypeName name, bool signed)
        : base(name, signed)
    {
        BareType = new(this, FixedList.Empty<DataType>());
        Type = BareType.With(Capability.Constant);
    }

    public override BareValueType<BigIntegerType> With(IFixedList<DataType> typeArguments)
    {
        RequiresEmpty(typeArguments);
        return BareType;
    }

    public override ValueType<BigIntegerType> With(Capability capability, IFixedList<DataType> typeArguments)
        => With(typeArguments).With(capability);

    public override ValueType<BigIntegerType> With(Capability capability)
        => BareType.With(capability);
}
