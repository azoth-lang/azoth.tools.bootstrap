using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;

public sealed class SelfType : BareTypeVariableType
{
    public DeclaredType ContainingType { get; }
    public override TypeName Name => SpecialTypeName.Self;

    public SelfType(DeclaredType containingType)
    {
        ContainingType = containingType;
    }

    public override CapabilityType With(Capability capability)
        => throw new NotImplementedException();

    public override CapabilityType WithRead()
        => With(ContainingType.IsDeclaredConst ? Capability.Constant : Capability.Read);

    #region Equality
    public override bool Equals(BareType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is SelfType otherType
               && ContainingType == otherType.ContainingType;
    }

    public override int GetHashCode() => HashCode.Combine(ContainingType, Name);
    #endregion

    public override string ToILString() => "Self";

    public override string ToSourceCodeString() => "Self";
}
