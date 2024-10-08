using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Declared;

[Closed(
    typeof(BoolType),
    typeof(NumericType))]
public abstract class SimpleType : DeclaredValueType
{
    public override IdentifierName? ContainingPackage => null;
    public override NamespaceName ContainingNamespace => NamespaceName.Global;

    public override SpecialTypeName Name { get; }

    public override IFixedSet<BareReferenceType> Supertypes => [];

    public abstract BareValueType BareType { get; }

    public abstract CapabilityType Type { get; }

    private protected SimpleType(SpecialTypeName name)
        : base(isConstType: true, [])
    {
        Name = name;
    }

    public abstract override BareValueType With(IFixedList<IType> typeArguments);

    public abstract override CapabilityType With(Capability capability, IFixedList<IType> typeArguments);

    public abstract CapabilityType With(Capability capability);

    #region Equals
    public override bool Equals(DeclaredType? other)
        // Most simple types are fixed instances, so a reference comparision suffices
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(Name);
    #endregion

    public override string ToString() => Name.ToString();
}
