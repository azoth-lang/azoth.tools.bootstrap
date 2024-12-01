using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;

public sealed class BigIntegerType : IntegerType
{
    internal new static readonly BigIntegerType Int = new(SpecialTypeName.Int, true);
    internal new static readonly BigIntegerType UInt = new(SpecialTypeName.UInt, false);

    public override BareValueType<BigIntegerType> BareType { get; }

    public override CapabilityType<BigIntegerType> Type { get; }

    private BigIntegerType(SpecialTypeName name, bool isSigned)
        : base(name, isSigned)
    {
        BareType = new(this, []);
        Type = BareType.With(Capability.Constant);
    }

    public override BareValueType<BigIntegerType> With(IFixedList<IType> typeArguments)
    {
        RequiresEmpty(typeArguments);
        return BareType;
    }

    public override CapabilityType<BigIntegerType> With(Capability capability, IFixedList<IType> typeArguments)
        => With(typeArguments).With(capability);

    public override CapabilityType<BigIntegerType> With(Capability capability)
        => BareType.With(capability);

    public override ITypeConstructor ToTypeConstructor() => IsSigned ? IAntetype.Int : IAntetype.UInt;
}
