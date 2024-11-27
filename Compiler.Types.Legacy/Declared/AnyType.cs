using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Declared;

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
    public override SpecialTypeName Name => SpecialTypeName.Any;
    public override IFixedSet<BareReferenceType> Supertypes => [];

    public BareReferenceType<AnyType> BareType { get; }

    public override BareReferenceType<AnyType> With(IFixedList<IType> typeArguments)
    {
        RequiresEmpty(typeArguments);
        return BareType;
    }

    public override CapabilityType<AnyType> With(Capability capability, IFixedList<IType> typeArguments)
        => With(typeArguments).With(capability);

    public CapabilityType<AnyType> With(Capability capability)
        => CapabilityType.Create(capability, BareType);

    public override IDeclaredAntetype ToAntetype() => IAntetype.Any;

    #region Equals
    public override bool Equals(DeclaredType? other) => ReferenceEquals(this, other);

    public override int GetHashCode() => Name.GetHashCode();
    #endregion

    public override string ToString() => Name.ToString();
}
