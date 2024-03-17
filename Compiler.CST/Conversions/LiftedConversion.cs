using System;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions;

/// <summary>
/// Lifts a conversion from T to T' to the optional types T? to T'?
/// </summary>
public sealed class LiftedConversion : ChainedConversion
{
    public Conversion UnderlyingConversion { get; }

    public LiftedConversion(Conversion underlyingConversion, Conversion priorConversion) : base(priorConversion)
    {
        UnderlyingConversion = underlyingConversion;
    }

    public override DataType Apply(DataType type)
    {
        type = PriorConversion.Apply(type);
        if (type is not OptionalType optionalType)
            throw new InvalidOperationException($"Cannot apply lifted conversion to non-optional type '{type}'");
        DataType newType = UnderlyingConversion.Apply(optionalType);
        return new OptionalType(newType);
    }
}
