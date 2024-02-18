using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Declared;

[Closed(
    //typeof(FloatingPointType),
    typeof(IntegerType))]
public abstract class NumericType : SimpleType, INumericType
{
    private protected NumericType(SpecialTypeName name)
        : base(name)
    {
    }
}
