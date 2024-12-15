using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

public sealed class PointerSizedIntegerTypeConstructor : IntegerTypeConstructor
{
    internal new static readonly PointerSizedIntegerTypeConstructor Size = new(BuiltInTypeName.Size, false);
    internal new static readonly PointerSizedIntegerTypeConstructor Offset = new(BuiltInTypeName.Offset, true);
    internal new static readonly PointerSizedIntegerTypeConstructor NInt = new(BuiltInTypeName.NInt, true);
    internal new static readonly PointerSizedIntegerTypeConstructor NUInt = new(BuiltInTypeName.NUInt, false);

    private PointerSizedIntegerTypeConstructor(BuiltInTypeName name, bool isSigned)
        : base(name, isSigned)
    {
    }

    /// <summary>
    /// The current type but signed.
    /// </summary>
    // TODO it could be a problem that not all values of `size` fit into the with sign type `offset`.
    // TODO it could be a problem that not all values of `nuint` fit into the with sign type `nint`.
    public override IntegerTypeConstructor WithSign()
    {
        if (IsSigned) return this;
        if (ReferenceEquals(this, Size)) return Offset;
        if (ReferenceEquals(this, NUInt)) return NUInt;
        throw new UnreachableException("All types should be covered");
    }
}
