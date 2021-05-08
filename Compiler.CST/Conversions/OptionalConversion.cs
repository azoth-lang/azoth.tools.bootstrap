using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions
{
    /// <summary>
    /// A conversion from `T` to `T?`
    /// </summary>
    public class OptionalConversion : Conversion
    {
        public new OptionalType To { [DebuggerStepThrough] get; }

        public OptionalConversion(OptionalType to)
            : base(to)
        {
            To = to;
        }
    }
}
