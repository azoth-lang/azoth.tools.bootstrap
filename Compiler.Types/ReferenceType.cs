using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public sealed class ReferenceType<TDeclared> : CapabilityType<TDeclared>
    where TDeclared : DeclaredReferenceType
{
    public override BareReferenceType<TDeclared> BareType { get; }

    internal ReferenceType(Capability capability, BareReferenceType<TDeclared> bareType)
        : base(capability)
    {
        if (typeof(TDeclared).IsAbstract)
            throw new ArgumentException($"The type parameter must be a concrete {nameof(DeclaredReferenceType)}.", nameof(TDeclared));
        BareType = bareType;
    }

    public override ReferenceType<TDeclared> With(Capability capability)
    {
        if (capability == Capability) return this;
        return new(capability, BareType);
    }
}
