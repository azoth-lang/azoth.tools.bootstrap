using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions
{
    public sealed class RecoverConst : RecoverConversion
    {
        public RecoverConst(Conversion priorConversion)
            : base(priorConversion)
        {
        }

        public override (DataType, ExpressionSemantics) Apply(DataType type, ExpressionSemantics semantics)
        {
            (type, _) = PriorConversion.Apply(type, semantics);
            if (type is not ReferenceType referenceType)
                throw new InvalidOperationException($"Cannot recover const for type '{type}'");
            return (referenceType.To(ReferenceCapability.Constant), ExpressionSemantics.ConstReference);
        }
    }
}
