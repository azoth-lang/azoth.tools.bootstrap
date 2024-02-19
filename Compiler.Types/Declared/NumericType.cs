using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Declared;

[Closed(
    //typeof(FloatingPointType),
    typeof(IntegerType))]
public abstract class NumericType : SimpleType, INumericType
{
    DataType INumericType.Type => Type;

    private protected NumericType(SpecialTypeName name)
        : base(name)
    {
    }
}
