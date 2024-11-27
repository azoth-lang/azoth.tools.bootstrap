using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.ConstValue;

[Closed(typeof(NumericType), typeof(IntegerConstValueType))]
internal interface INumericType
{
    IExpressionType Type { get; }
}
