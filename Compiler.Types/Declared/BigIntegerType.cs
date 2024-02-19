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
        BareType = new(this, FixedList<DataType>.Empty);
        Type = BareType.With(ReferenceCapability.Constant);
    }

    public override BareValueType<BigIntegerType> With(IFixedList<DataType> typeArguments)
    {
        RequiresEmpty(typeArguments);
        return BareType;
    }

    public override ValueType<BigIntegerType> With(ReferenceCapability capability, IFixedList<DataType> typeArguments)
        => With(typeArguments).With(capability);

    public override ValueType<BigIntegerType> With(ReferenceCapability capability)
        => BareType.With(capability);
}
