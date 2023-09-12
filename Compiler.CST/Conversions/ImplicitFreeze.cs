using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions;

public class ImplicitFreeze : ChainedConversion
{
    public ImplicitFreeze(Conversion priorConversion)
        : base(priorConversion) { }

    public override (DataType, ExpressionSemantics) Apply(DataType type, ExpressionSemantics semantics)
    {
        (type, _) = PriorConversion.Apply(type, semantics);
        if (type is not ReferenceType { AllowsFreeze: true } referenceType)
            throw new InvalidOperationException($"Cannot implicitly freeze type '{type.ToILString()}'");
        return (referenceType.With(ReferenceCapability.Constant), ExpressionSemantics.ConstReference);
    }
}
