using System;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions
{
    public class RecoverConst : RecoverConversion
    {
        public RecoverConst(Conversion underlyingConversion, ReferenceType to)
            : base(underlyingConversion, to)
        {
            if (to.Capability != ReferenceCapability.Constant)
                throw new ArgumentException("Recover to constant must produce a constant type", nameof(to));
        }
    }
}
