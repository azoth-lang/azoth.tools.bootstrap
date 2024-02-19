using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Declared;

public sealed class DeclaredAnyType : DeclaredReferenceType
{
    #region Singleton
    internal static readonly DeclaredAnyType Instance = new();

    private DeclaredAnyType()
        : base(isConstType: false, isAbstract: true, FixedList<GenericParameterType>.Empty)
    {
        BareType = new(this, FixedList<DataType>.Empty);
    }
    #endregion

    public override SimpleName? ContainingPackage => null;
    public override NamespaceName ContainingNamespace => NamespaceName.Global;
    public override SpecialTypeName Name => SpecialTypeName.Any;
    public override FixedSet<BareReferenceType> Supertypes => FixedSet<BareReferenceType>.Empty;

    public BareReferenceType<DeclaredAnyType> BareType { get; }

    public override BareReferenceType<DeclaredAnyType> With(FixedList<DataType> typeArguments)
    {
        RequiresEmpty(typeArguments);
        return BareType;
    }

    public override ReferenceType<DeclaredAnyType> With(ReferenceCapability capability, FixedList<DataType> typeArguments)
        => With(typeArguments).With(capability);

    public ReferenceType<DeclaredAnyType> With(ReferenceCapability capability)
        => ReferenceType.Create(capability, BareType);

    #region Equals
    public override bool Equals(DeclaredType? other) => ReferenceEquals(this, other);

    public override int GetHashCode() => Name.GetHashCode();
    #endregion

    public override string ToString() => Name.ToString();
}
