using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Declared;

/// <summary>
/// A reference type as it is declared without any capability or type arguments
/// </summary>
[Closed(
    typeof(DeclaredObjectType),
    typeof(DeclaredAnyType))]
public abstract class DeclaredReferenceType : DeclaredType, IEquatable<DeclaredReferenceType>
{
    public bool IsAbstract { get; }

    protected DeclaredReferenceType(
        bool isConstType,
        bool isAbstract,
        FixedList<GenericParameterType> genericParametersTypes)
        : base(isConstType, genericParametersTypes)
    {
        IsAbstract = isAbstract;
    }

    public abstract override BareReferenceType With(FixedList<DataType> typeArguments);

    public abstract override ReferenceType With(ReferenceCapability capability, FixedList<DataType> typeArguments);

    #region Equality
    public abstract bool Equals(DeclaredReferenceType? other);

    public override bool Equals(object? obj)
        => obj is DeclaredReferenceType other && Equals(other);

    public abstract override int GetHashCode();

    public static bool operator ==(DeclaredReferenceType? left, DeclaredReferenceType? right) => Equals(left, right);

    public static bool operator !=(DeclaredReferenceType? left, DeclaredReferenceType? right) => !Equals(left, right);
    #endregion
}
