using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Declared;

public sealed class BoolType : SimpleType
{
    #region Singleton
    internal static readonly BoolType Instance = new();

    private BoolType()
        : base(SpecialTypeName.Bool)
    {
        BareType = new(this, FixedList.Empty<DataType>());
        Type = BareType.With(Capability.Constant);
    }
    #endregion

    public override BareValueType<BoolType> BareType { get; }

    public override ValueType<BoolType> Type { get; }

    public override BareValueType<BoolType> With(IFixedList<DataType> typeArguments)
    {
        RequiresEmpty(typeArguments);
        return BareType;
    }

    public override ValueType<BoolType> With(Capability capability, IFixedList<DataType> typeArguments)
        => With(typeArguments).With(capability);

    public override ValueType<BoolType> With(Capability capability)
        => BareType.With(capability);
}
