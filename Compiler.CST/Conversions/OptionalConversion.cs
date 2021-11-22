using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions
{
    /// <summary>
    /// A conversion from `T` to `T?`
    /// </summary>
    public class OptionalConversion : Conversion
    {
        public Conversion UnderlyingConversion { [DebuggerStepThrough] get; }
        public new OptionalType To { [DebuggerStepThrough] get; }

        public OptionalConversion(Conversion underlyingConversion)
            : base(new OptionalType(underlyingConversion.To))
        {
            UnderlyingConversion = underlyingConversion;
            To = (OptionalType)base.To;
        }
    }
}
