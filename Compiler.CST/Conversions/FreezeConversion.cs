using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions;

public class FreezeConversion : ChainedConversion
{
    public ConversionKind Kind { get; }

    public FreezeConversion(Conversion priorConversion, ConversionKind kind)
        : base(priorConversion)
    {
        Kind = kind;
    }

    public override (DataType, ExpressionSemantics) Apply(DataType type, ExpressionSemantics semantics)
    {
        (type, _) = PriorConversion.Apply(type, semantics);
        if (type is not ReferenceType { AllowsFreeze: true } referenceType)
            throw new InvalidOperationException($"Cannot freeze type '{type.ToILString()}'");
        var capability = Kind == ConversionKind.Temporary
            ? ReferenceCapability.TemporarilyConstant
            : ReferenceCapability.Constant;
        return (referenceType.With(capability), ExpressionSemantics.ConstReference);
    }
}
