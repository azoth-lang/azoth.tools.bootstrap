using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
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

    public override (DataType, ExpressionSemantics) Apply(DataType type, ExpressionSemantics semantics)
    {
        (type, semantics) = PriorConversion.Apply(type, semantics);
        if (type is not OptionalType optionalType)
            throw new InvalidOperationException($"Cannot apply lifted conversion to non-optional type '{type}'");
        DataType? newType;
        (newType, semantics) = UnderlyingConversion.Apply(optionalType, semantics);
        return (new OptionalType(newType), semantics);
    }
}
