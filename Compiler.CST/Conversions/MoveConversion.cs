using System;
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

    public override DataType Apply(DataType type)
    {
        type = PriorConversion.Apply(type);
        if (type is not CapabilityType { AllowsRecoverIsolation: true } referenceType)
            throw new InvalidOperationException($"Cannot move type '{type.ToILString()}'");
        var capability = Kind == ConversionKind.Temporary
            ? Capability.TemporarilyIsolated
            : Capability.Isolated;
        return referenceType.With(capability);
    }
}
