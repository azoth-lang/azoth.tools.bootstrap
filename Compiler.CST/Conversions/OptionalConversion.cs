using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions;

/// <summary>
/// A conversion from `T` to `T?`
/// </summary>
public sealed class OptionalConversion : ChainedConversion
{
    public OptionalConversion(Conversion priorConversion) : base(priorConversion)
    {
    }

    public override (DataType, ExpressionSemantics) Apply(DataType type, ExpressionSemantics semantics)
    {
        (type, semantics) = PriorConversion.Apply(type, semantics);
        return (new OptionalType(type), semantics);
    }
}
