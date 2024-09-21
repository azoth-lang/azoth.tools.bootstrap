using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;
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
    internal new static readonly PointerSizedIntegerType NInt = new(SpecialTypeName.NInt, true);
    internal new static readonly PointerSizedIntegerType NUInt = new(SpecialTypeName.NUInt, false);

    public override BareValueType<PointerSizedIntegerType> BareType { get; }

    public override CapabilityType<PointerSizedIntegerType> Type { get; }

    private PointerSizedIntegerType(SpecialTypeName name, bool signed)
        : base(name, signed)
    {
        BareType = new(this, []);
        Type = BareType.With(Capability.Constant);
    }

    /// <summary>
    /// The current type but signed.
    /// </summary>
    public IntegerType WithSign()
    {
        if (IsSigned) return this;
        if (this == Size) return Offset;
        if (this == NUInt) return NInt;
        throw new UnreachableException("All types should be covered");
    }

    public override BareValueType<PointerSizedIntegerType> With(IFixedList<IType> typeArguments)
    {
        RequiresEmpty(typeArguments);
        return BareType;
    }

    public override CapabilityType<PointerSizedIntegerType> With(Capability capability, IFixedList<IType> typeArguments)
        => With(typeArguments).With(capability);


    public override CapabilityType<PointerSizedIntegerType> With(Capability capability)
        => BareType.With(capability);

    public override IDeclaredAntetype ToAntetype()
    {
        if (this == Size) return IAntetype.Size;
        if (this == Offset) return IAntetype.Offset;
        if (this == NInt) return IAntetype.NInt;
        if (this == NUInt) return IAntetype.NUInt;
        throw new UnreachableException();
    }
}
