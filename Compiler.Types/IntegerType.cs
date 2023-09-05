using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(
    typeof(IntegerConstantType),
    typeof(FixedSizeIntegerType),
    typeof(PointerSizedIntegerType))]
public abstract class IntegerType : NumericType
{
    public bool IsSigned { get; }

    private protected IntegerType(SpecialTypeName name, bool isSigned)
        : base(name)
    {
        IsSigned = isSigned;
    }
}
