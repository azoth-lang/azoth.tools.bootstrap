using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;

[Closed(typeof(NumericType), typeof(IntegerConstValueType))]
internal interface INumericType
{
    IExpressionType Type { get; }
}
