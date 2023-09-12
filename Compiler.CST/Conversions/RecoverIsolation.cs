using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions;

public sealed class RecoverIsolation : RecoverConversion
{
    public RecoverIsolation(Conversion priorConversion)
        : base(priorConversion)
    {
    }

    public override (DataType, ExpressionSemantics) Apply(DataType type, ExpressionSemantics semantics)
    {
        (type, _) = PriorConversion.Apply(type, semantics);
        if (type is not ReferenceType { AllowsRecoverIsolation: true } referenceType)
            throw new InvalidOperationException($"Cannot recover isolation for type '{type.ToILString()}'");
        return (referenceType.With(ReferenceCapability.Isolated), ExpressionSemantics.IsolatedReference);
    }
}
