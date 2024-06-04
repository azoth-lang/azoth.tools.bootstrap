using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Compiler.Antetypes;

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

}
