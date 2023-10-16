using System;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public sealed class DeclaredAnyType : DeclaredReferenceType
{
    #region Singleton
    internal static readonly DeclaredAnyType Instance = new();

    private DeclaredAnyType() : base(true) { }
    #endregion

    public override SimpleName? ContainingPackage => null;
    public override NamespaceName ContainingNamespace => NamespaceName.Global;
    public override Name Name => SpecialTypeName.Any;

    public override AnyType With(ReferenceCapability capability, FixedList<DataType> typeArguments)
    {
        if (typeArguments.Count != 0)
            throw new ArgumentException($"`{SpecialTypeName.Any}` does not support type arguments.");
        return new(capability);
    }

    [SuppressMessage("Performance", "CA1822:Mark members as static",
        Justification = "OO")]
    public AnyType With(ReferenceCapability capability) => new(capability);

    public override bool IsAssignableFrom(DeclaredReferenceType source) => true;

    #region Equals
    public override bool Equals(DeclaredReferenceType? other) => ReferenceEquals(this, other);

    public override int GetHashCode() => Name.GetHashCode();
    #endregion

    public override string ToString() => Name.ToString();
}
