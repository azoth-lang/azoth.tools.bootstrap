using System;
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

    public override DataType Apply(DataType type)
    {
        type = PriorConversion.Apply(type);
        if (type is not CapabilityType { AllowsFreeze: true } referenceType)
            throw new InvalidOperationException($"Cannot freeze type '{type.ToILString()}'");
        var capability = Kind == ConversionKind.Temporary
            ? Capability.TemporarilyConstant
            : Capability.Constant;
        return referenceType.With(capability);
    }
}
