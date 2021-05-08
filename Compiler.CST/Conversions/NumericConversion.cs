using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions
{
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
