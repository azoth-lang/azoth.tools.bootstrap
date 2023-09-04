using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions;

/// <summary>
/// Conversion between numeric types. For example `int32` to `int64`.
/// </summary>
public sealed class NumericConversion : ChainedConversion
{
    public NumericType To { [DebuggerStepThrough] get; }

    public NumericConversion(NumericType to, Conversion priorConversion)
        : base(priorConversion)
    {
        To = to;
    }

    public override (DataType, ExpressionSemantics) Apply(DataType type, ExpressionSemantics semantics)
    {
        (type, semantics) = PriorConversion.Apply(type, semantics);
        // TODO check that the incoming type and semantics can work with numeric conversion
        return (To, ExpressionSemantics.CopyValue);
    }
}
