using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;

public sealed class AnyType : DeclaredReferenceType
{
    #region Singleton
    internal static readonly AnyType Instance = new();

    private AnyType()
        : base(isDeclaredConst: false, isAbstract: true, isClass: false, [])
    {
        BareType = new(this, []);
    }
    #endregion

    public override IdentifierName? ContainingPackage => null;
    public override NamespaceName ContainingNamespace => NamespaceName.Global;
    public override TypeSemantics Semantics => TypeSemantics.Reference;
    public override bool CanHaveFields => false;
    public override SpecialTypeName Name => SpecialTypeName.Any;
    public override IFixedSet<BareNonVariableType> Supertypes => [];

    public BareNonVariableType BareType { get; }

    public override BareNonVariableType With(IFixedList<IType> typeArguments)
    {
        RequiresEmpty(typeArguments);
        return BareType;
    }

    public override CapabilityType With(Capability capability, IFixedList<IType> typeArguments)
        => With(typeArguments).With(capability);

    public CapabilityType With(Capability capability)
        => CapabilityType.Create(capability, BareType);

    public override TypeConstructor? ToTypeConstructor() => TypeConstructor.Any;
    public override IPlainType TryToPlainType() => IPlainType.Any;

    #region Equals
    public override bool Equals(DeclaredType? other) => ReferenceEquals(this, other);

    public override int GetHashCode() => Name.GetHashCode();
    #endregion

    public override string ToString() => Name.ToString();
}
