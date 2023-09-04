using System.Diagnostics;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions;

/// <summary>
/// A chained conversion is one that is applied after another conversion, even if
/// that is only is only the identity conversion.
/// </summary>
[Closed(
    typeof(OptionalConversion),
    typeof(NumericConversion),
    typeof(LiftedConversion),
    typeof(RecoverConversion))]
public abstract class ChainedConversion : Conversion
{
    public Conversion PriorConversion { [DebuggerStepThrough] get; }

    protected ChainedConversion(Conversion priorConversion)
    {
        PriorConversion = priorConversion;
    }

    public override bool IsChainedTo(Conversion conversion)
        => PriorConversion == conversion || PriorConversion.IsChainedTo(conversion);
}
