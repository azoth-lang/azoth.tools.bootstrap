using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions
{
    /// <summary>
    /// Conversion between numeric types. For example `int32` to `int64`.
    /// </summary>
    public class NumericConversion : Conversion
    {
        public new NumericType To { [DebuggerStepThrough] get; }

        public NumericConversion(NumericType to)
            : base(to)
        {
            To = to;
        }
    }
}
