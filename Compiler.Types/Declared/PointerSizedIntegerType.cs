using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Declared;

/// <summary>
/// Integer types whose exact bit length is architecture dependent and whose
/// length matches that of pointers.
/// </summary>
public sealed class PointerSizedIntegerType : IntegerType
{
    internal new static readonly PointerSizedIntegerType Size = new(SpecialTypeName.Size, false);
    internal new static readonly PointerSizedIntegerType Offset = new(SpecialTypeName.Offset, true);

    public override BareValueType<PointerSizedIntegerType> BareType { get; }

    public override ValueType<PointerSizedIntegerType> Type { get; }

    private PointerSizedIntegerType(SpecialTypeName name, bool signed)
        : base(name, signed)
    {
        BareType = new(this, FixedList.Empty<DataType>());
        Type = BareType.With(ReferenceCapability.Constant);
    }

    public override BareValueType<PointerSizedIntegerType> With(IFixedList<DataType> typeArguments)
    {
        RequiresEmpty(typeArguments);
        return BareType;
    }

    public override ValueType<PointerSizedIntegerType> With(ReferenceCapability capability, IFixedList<DataType> typeArguments)
        => With(typeArguments).With(capability);

    public override ValueType<PointerSizedIntegerType> With(ReferenceCapability capability)
        => BareType.With(capability);
}
