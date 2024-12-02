using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

[Closed(
    typeof(PointerSizedIntegerTypeConstructor),
    typeof(FixedSizeIntegerTypeConstructor),
    typeof(BigIntegerTypeConstructor))]
public abstract class IntegerTypeConstructor : NumericTypeConstructor
{
    public bool IsSigned { get; }

    protected IntegerTypeConstructor(SpecialTypeName name, bool isSigned)
        : base(name)
    {
        IsSigned = isSigned;
    }

    /// <summary>
    /// The current type but signed.
    /// </summary>
    public abstract IntegerTypeConstructor WithSign();
}
