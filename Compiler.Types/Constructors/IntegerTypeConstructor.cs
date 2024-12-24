using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

[Closed(
    typeof(PointerSizedIntegerTypeConstructor),
    typeof(FixedSizeIntegerTypeConstructor),
    typeof(BigIntegerTypeConstructor))]
public abstract class IntegerTypeConstructor : SimpleTypeConstructor
{
    public bool IsSigned { [DebuggerStepThrough] get; }

    protected IntegerTypeConstructor(BuiltInTypeName name, bool isSigned)
        : base(name)
    {
        IsSigned = isSigned;
    }

    /// <summary>
    /// The current type but signed.
    /// </summary>
    public abstract IntegerTypeConstructor WithSign();
}
