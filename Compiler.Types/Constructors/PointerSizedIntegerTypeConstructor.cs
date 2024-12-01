using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

public sealed class PointerSizedIntegerTypeConstructor : IntegerTypeConstructor
{
    internal static readonly PointerSizedIntegerTypeConstructor Size = new(SpecialTypeName.Size, false);
    internal static readonly PointerSizedIntegerTypeConstructor Offset = new(SpecialTypeName.Offset, true);
    internal static readonly PointerSizedIntegerTypeConstructor NInt = new(SpecialTypeName.NInt, true);
    internal static readonly PointerSizedIntegerTypeConstructor NUInt = new(SpecialTypeName.NUInt, false);

    private PointerSizedIntegerTypeConstructor(SpecialTypeName name, bool isSigned)
        : base(name, isSigned)
    {
    }

    /// <summary>
    /// The current type but signed.
    /// </summary>
    public override IntegerTypeConstructor WithSign()
    {
        if (IsSigned) return this;
        if (ReferenceEquals(this, Size)) return Offset;
        if (ReferenceEquals(this, NUInt)) return NUInt;
        throw new UnreachableException("All types should be covered");
    }
}
