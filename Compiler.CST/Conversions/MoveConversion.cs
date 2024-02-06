using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions;

public class MoveConversion : ChainedConversion
{
    public ConversionKind Kind { get; }

    public MoveConversion(Conversion priorConversion, ConversionKind kind)
        : base(priorConversion)
    {
        Kind = kind;
    }

    public override (DataType, ExpressionSemantics) Apply(DataType type, ExpressionSemantics semantics)
    {
        (type, _) = PriorConversion.Apply(type, semantics);
        if (type is not ReferenceType { AllowsRecoverIsolation: true } referenceType)
            throw new InvalidOperationException($"Cannot move type '{type.ToILString()}'");
        var capability = Kind == ConversionKind.Temporary
            ? ReferenceCapability.TemporarilyIsolated
            : ReferenceCapability.Isolated;
        return (referenceType.With(capability), ExpressionSemantics.IsolatedReference);
    }
}
