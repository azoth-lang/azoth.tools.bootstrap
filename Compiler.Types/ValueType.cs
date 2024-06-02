using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public sealed class ValueType<TDeclared> : CapabilityType<TDeclared>
    where TDeclared : DeclaredValueType
{
    public override BareValueType<TDeclared> BareType { get; }

    internal ValueType(Capability capability, BareValueType<TDeclared> bareType)
        : base(capability)
    {
        if (typeof(TDeclared).IsAbstract)
            throw new ArgumentException($"The type parameter must be a concrete {nameof(DeclaredValueType)}.", nameof(TDeclared));
        BareType = bareType;
    }

    public override ValueType<TDeclared> With(Capability capability)
    {
        if (capability == Capability) return this;
        return new(capability, BareType);
    }
}
