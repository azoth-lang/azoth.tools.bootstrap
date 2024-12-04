using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.ConstValue;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;

[Closed(
    //typeof(FloatingPointType),
    typeof(IntegerType))]
public abstract class NumericType : SimpleType, INumericType
{
    IType INumericType.Type => Type;

    private protected NumericType(SpecialTypeName name)
        : base(name)
    {
    }
}
