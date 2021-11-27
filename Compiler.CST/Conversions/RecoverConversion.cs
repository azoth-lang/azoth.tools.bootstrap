using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions
{
    /// <summary>
    /// A "conversion" that recovers a reference capability.
    /// </summary>
    [Closed(
        typeof(RecoverIsolation),
        typeof(RecoverConst))]
    public abstract class RecoverConversion : ChainedConversion
    {
        protected RecoverConversion(Conversion priorConversion) : base(priorConversion)
        {
        }
    }
}
