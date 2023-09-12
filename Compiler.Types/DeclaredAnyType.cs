using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public sealed class DeclaredAnyType : DeclaredReferenceType
{
    #region Singleton
    internal static readonly DeclaredAnyType Instance = new();

    private DeclaredAnyType() { }
    #endregion

    public override TypeName Name => SpecialTypeName.Any;

    public override AnyType With(ReferenceCapability capability, FixedList<DataType> typeArguments)
    {
        if (typeArguments.Count != 0)
            throw new ArgumentException($"`{SpecialTypeName.Any}` does not support type arguments.");
        return new(capability);
    }

    public AnyType With(ReferenceCapability capability) => new(capability);

    #region Equals
    public override bool Equals(DeclaredReferenceType? other) => ReferenceEquals(this, other);

    public override int GetHashCode() => Name.GetHashCode();
    #endregion

    public override string ToString() => Name.ToString();
}
