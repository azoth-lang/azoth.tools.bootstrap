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
        typeof(OptionalConversion),
        typeof(NumericConversion),
        typeof(ImmutabilityConversion), // TODO not sure this should be a conversion
        typeof(LiftedConversion))]
    public abstract class Conversion
    {
        public DataType To { [DebuggerStepThrough] get; }

        protected Conversion(DataType to)
        {
            To = to;
        }
    }
}
