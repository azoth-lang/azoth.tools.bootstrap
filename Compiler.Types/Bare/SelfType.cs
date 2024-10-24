using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

public sealed class SelfType : BareTypeVariableType
{
    public DeclaredType ContainingType { get; }

    public SelfType(DeclaredType containingType)
    {
        ContainingType = containingType;
    }

    public override CapabilityType With(Capability capability)
        => throw new NotImplementedException();

    public override CapabilityType WithRead()
        => With(ContainingType.IsDeclaredConst ? Capability.Constant : Capability.Read);

    public override string ToILString() => "Self";

    public override string ToSourceCodeString() => "Self";
}
