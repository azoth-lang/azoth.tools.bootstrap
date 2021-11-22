using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions
{
    /// <summary>
    /// A conversion represents an actual conversion between types. It doesn't come in to play for
    /// simple subtype relationships.
    /// </summary>
    [Closed(
        typeof(IdentityConversion),
        typeof(OptionalConversion),
        typeof(NumericConversion),
        typeof(ImmutabilityConversion),
        typeof(LiftedConversion),
        typeof(RecoverConversion))]
    public abstract class Conversion
    {
        public DataType To { [DebuggerStepThrough] get; }

        protected Conversion(DataType to)
        {
            To = to;
        }
    }
}
