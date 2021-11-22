using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions
{
    /// <summary>
    /// A "conversion" that recovers a reference capability.
    /// </summary>
    [Closed(
        typeof(RecoverIsolation),
        typeof(RecoverConst))]
    public abstract class RecoverConversion : Conversion
    {
        public Conversion UnderlyingConversion { [DebuggerStepThrough] get; }

        public new ReferenceType To { [DebuggerStepThrough] get; }

        protected RecoverConversion(Conversion underlyingConversion, ReferenceType to)
            : base(to)
        {
            UnderlyingConversion = underlyingConversion;
            To = to;
        }
    }
}
