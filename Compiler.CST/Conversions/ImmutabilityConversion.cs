using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions
{
    public class ImmutabilityConversion : Conversion
    {
        public new ObjectType To { [DebuggerStepThrough] get; }

        public ImmutabilityConversion(ObjectType to)
            : base(to)
        {
            To = to;
        }
    }
}
