using System;
using System.Diagnostics.CodeAnalysis;
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
    { }
    #endregion

    public override SimpleName? ContainingPackage => null;
    public override NamespaceName ContainingNamespace => NamespaceName.Global;
    public override SpecialTypeName Name => SpecialTypeName.Any;
    public override FixedSet<BareReferenceType> Supertypes => FixedSet<BareReferenceType>.Empty;

    public override BareReferenceType<DeclaredAnyType> With(FixedList<DataType> typeArguments)
    {
        if (typeArguments.Count != 0)
            throw new ArgumentException($"`{SpecialTypeName.Any}` does not support type arguments.");
        return BareReferenceType.Any;
    }

    public override ReferenceType<DeclaredAnyType> With(ReferenceCapability capability, FixedList<DataType> typeArguments)
        => With(typeArguments).With(capability);

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
    public ReferenceType<DeclaredAnyType> With(ReferenceCapability capability)
        => ReferenceType.Create(capability, BareReferenceType.Any);

    #region Equals
    public override bool Equals(DeclaredReferenceType? other) => ReferenceEquals(this, other);

    public override int GetHashCode() => Name.GetHashCode();
    #endregion

    public override string ToString() => Name.ToString();
}
