using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;

public sealed class BoolType : SimpleType
{
    #region Singleton
    internal static readonly BoolType Instance = new();

    private BoolType()
        : base(SpecialTypeName.Bool)
    {
        BareType = new(this, []);
        Type = BareType.With(Capability.Constant);
    }
    #endregion

    public override BareValueType<BoolType> BareType { get; }

    public override CapabilityType Type { get; }

    public override BareValueType<BoolType> With(IFixedList<IType> typeArguments)
    {
        RequiresEmpty(typeArguments);
        return BareType;
    }

    public override CapabilityType With(Capability capability, IFixedList<IType> typeArguments)
        => With(typeArguments).With(capability);

    public override CapabilityType With(Capability capability)
        => BareType.With(capability);

    public override TypeConstructor? ToTypeConstructor() => null;
    public override IPlainType TryToPlainType() => IPlainType.Bool;
}
