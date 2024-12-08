using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;

public sealed class SelfType : BareAssociatedType
{
    public TypeConstructor ContainingType { get; }
    public override TypeName Name => SpecialTypeName.Self;
    // TODO this should include the containing type and its supertypes
    public override IFixedSet<BareNonVariableType> Supertypes => [];

    public SelfType(TypeConstructor containingType)
    {
        ContainingType = containingType;
    }

    public override CapabilityType With(Capability capability)
        => throw new NotImplementedException();

    public override CapabilityType WithRead()
        => With(ContainingType.IsDeclaredConst ? Capability.Constant : Capability.Read);

    public override SelfPlainType ToPlainType() => new SelfPlainType(ContainingType);

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
