using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// A reference type without any capability attached
/// </summary>
[Closed(
    typeof(DeclaredObjectType),
    typeof(DeclaredAnyType))]
public abstract class DeclaredReferenceType : IEquatable<DeclaredReferenceType>
{
    public static readonly DeclaredAnyType Any = DeclaredAnyType.Instance;

    public abstract SimpleName? ContainingPackage { get; }

    public abstract NamespaceName ContainingNamespace { get; }

    public bool IsAbstract { get; }

    public abstract Name Name { get; }

    protected DeclaredReferenceType(bool isAbstract)
    {
        IsAbstract = isAbstract;
    }

    public abstract ReferenceType With(ReferenceCapability capability, FixedList<DataType> typeArguments);

    public abstract bool IsAssignableFrom(DeclaredReferenceType source);

    #region MyRegion
    public abstract bool Equals(DeclaredReferenceType? other);

    public override bool Equals(object? obj)
        => obj is DeclaredReferenceType other && Equals(other);

    public abstract override int GetHashCode();

    public static bool operator ==(DeclaredReferenceType? left, DeclaredReferenceType? right) => Equals(left, right);

    public static bool operator !=(DeclaredReferenceType? left, DeclaredReferenceType? right) => !Equals(left, right);
    #endregion

    public abstract override string ToString();
}
