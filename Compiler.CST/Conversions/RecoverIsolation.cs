using System;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions
{
    public class RecoverIsolation : RecoverConversion
    {
        public RecoverIsolation(Conversion underlyingConversion, ReferenceType to)
            : base(underlyingConversion, to)
        {
            if (to.Capability != ReferenceCapability.Isolated)
                throw new ArgumentException("Recover to isolated must produce a isolated type", nameof(to));

        }
    }
}
