using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions
{
    /// <summary>
    /// Lifts a conversion from T to T' to the optional types T? to T'?
    /// </summary>
    public class LiftedConversion : Conversion
    {
        public Conversion UnderlyingConversion { [DebuggerStepThrough] get; }
        public new OptionalType To { [DebuggerStepThrough] get; }

        public LiftedConversion(Conversion underlyingConversion)
            : base(new OptionalType(underlyingConversion.To))
        {
            UnderlyingConversion = underlyingConversion;
            To = (OptionalType)base.To;
        }
    }
}
