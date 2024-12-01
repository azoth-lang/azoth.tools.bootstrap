using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

[Closed(
    typeof(PointerSizedIntegerAntetype),
    typeof(FixedSizeIntegerAntetype),
    typeof(BigIntegerAntetype))]
public abstract class IntegerAntetype : NumericAntetype
{
    public bool IsSigned { get; }

    private protected IntegerAntetype(SpecialTypeName name, bool isSigned)
        : base(name)
    {
        IsSigned = isSigned;
    }
}
