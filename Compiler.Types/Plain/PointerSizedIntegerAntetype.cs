using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public sealed class PointerSizedIntegerAntetype : IntegerAntetype
{
    internal static readonly PointerSizedIntegerAntetype Size = new(SpecialTypeName.Size, false);
    internal static readonly PointerSizedIntegerAntetype Offset = new(SpecialTypeName.Offset, true);
    internal static readonly PointerSizedIntegerAntetype NInt = new(SpecialTypeName.NInt, true);
    internal static readonly PointerSizedIntegerAntetype NUInt = new(SpecialTypeName.NUInt, false);

    private PointerSizedIntegerAntetype(SpecialTypeName name, bool signed)
        : base(name, signed)
    {
    }

    /// <summary>
    /// The current type but signed.
    /// </summary>
    public IAntetype WithSign()
    {
        if (IsSigned) return this;
        if (ReferenceEquals(this, Size)) return Offset;
        if (ReferenceEquals(this, NUInt)) return NUInt;
        throw new UnreachableException("All types should be covered");
    }
}
