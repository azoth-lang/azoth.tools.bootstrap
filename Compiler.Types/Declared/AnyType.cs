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
        : base(isDeclaredConst: false, isAbstract: true, isClass: false, FixedList.Empty<GenericParameterType>())
    {
        BareType = new(this, FixedList.Empty<DataType>());
    }
    #endregion

    public override IdentifierName? ContainingPackage => null;
    public override NamespaceName ContainingNamespace => NamespaceName.Global;
    public override SpecialTypeName Name => SpecialTypeName.Any;
    public override FixedSet<BareReferenceType> Supertypes => FixedSet<BareReferenceType>.Empty;

    public BareReferenceType<AnyType> BareType { get; }

    public override BareReferenceType<AnyType> With(IFixedList<DataType> typeArguments)
    {
        RequiresEmpty(typeArguments);
        return BareType;
    }

    public override ReferenceType<AnyType> With(Capability capability, IFixedList<DataType> typeArguments)
        => With(typeArguments).With(capability);

    public ReferenceType<AnyType> With(Capability capability)
        => ReferenceType.Create(capability, BareType);

    #region Equals
    public override bool Equals(DeclaredType? other) => ReferenceEquals(this, other);

    public override int GetHashCode() => Name.GetHashCode();
    #endregion

    public override string ToString() => Name.ToString();
}
