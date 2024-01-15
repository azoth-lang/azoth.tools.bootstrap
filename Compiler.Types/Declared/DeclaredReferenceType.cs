using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Declared;

/// <summary>
/// A reference type as it is declared without any capability or type arguments
/// </summary>
[Closed(
    typeof(DeclaredObjectType),
    typeof(DeclaredAnyType))]
public abstract class DeclaredReferenceType : IEquatable<DeclaredReferenceType>
{
    public static readonly DeclaredAnyType Any = DeclaredAnyType.Instance;

    public abstract SimpleName? ContainingPackage { get; }

    public abstract NamespaceName ContainingNamespace { get; }

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// const.
    /// </summary>
    public bool IsConstType { get; }

    public bool IsAbstract { get; }

    public abstract Name Name { get; }

    public FixedList<GenericParameter> GenericParameters { get; }

    public FixedList<GenericParameterType> GenericParameterTypes { get; }

    // TODO this is really awkward. There should be a subtype relationship
    public FixedList<DataType> GenericParameterDataTypes { get; }

    public abstract FixedSet<BareReferenceType> Supertypes { get; }

    protected DeclaredReferenceType(
        bool isConstType,
        bool isAbstract,
        FixedList<GenericParameterType> genericParametersTypes)
    {
        IsConstType = isConstType;
        IsAbstract = isAbstract;
        GenericParameters = genericParametersTypes.Select(t => t.Parameter).ToFixedList();
        GenericParameterTypes = genericParametersTypes;
        GenericParameterDataTypes = GenericParameterTypes.ToFixedList<DataType>();
    }

    public abstract ReferenceType With(ReferenceCapability capability, FixedList<DataType> typeArguments);

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
