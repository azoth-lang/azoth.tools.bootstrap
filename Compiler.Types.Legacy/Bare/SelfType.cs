using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;

public sealed class SelfType : BareType
{
    public TypeConstructor ContainingType { get; }
    public override TypeName Name => SpecialTypeName.Self;
    // TODO this should include the containing type and its supertypes
    public override IFixedSet<BareNonVariableType> Supertypes => [];
    public sealed override TypeConstructor? TypeConstructor => null;

    /// <remarks>Associated types are effectively not declared const because they could be inhabited
    /// by non-const types.</remarks>
    public override bool IsDeclaredConst => false;

    public override bool AllowsVariance => false;
    public override bool HasIndependentTypeArguments => false;
    public sealed override IFixedList<IType> TypeArguments => [];
    public sealed override IEnumerable<GenericParameterArgument> GenericParameterArguments => [];

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
    public override IType ReplaceTypeParametersIn(IType type) => type;
    public override IMaybeType ReplaceTypeParametersIn(IMaybeType type) => type;
    public override IPseudotype ReplaceTypeParametersIn(IPseudotype pseudotype) => pseudotype;
    public override IMaybePseudotype ReplaceTypeParametersIn(IMaybePseudotype pseudotype) => pseudotype;
    public override SelfType AccessedVia(Capability capability) => this;

    public override SelfType With(IFixedList<IType> typeArguments)
    {
        Requires.That(typeArguments.IsEmpty, nameof(typeArguments), "Cannot apply type arguments to associated type.");
        return this;
    }
}
