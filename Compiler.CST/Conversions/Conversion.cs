using Azoth.Tools.Bootstrap.Compiler.Core;
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
        typeof(ChainedConversion))]
    public abstract class Conversion
    {
        public abstract (DataType, ExpressionSemantics) Apply(DataType type, ExpressionSemantics semantics);
    }
}
