using System;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

/// <summary>
/// An reference type without a reference capability.
/// </summary>
[Closed(typeof(BareObjectType), typeof(BareAnyType))]
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public abstract class BareReferenceType : IEquatable<BareReferenceType>
{
    public static readonly BareAnyType Any = BareAnyType.Instance;

    public abstract DeclaredReferenceType DeclaredType { get; }

    public bool AllowsVariance => DeclaredType.AllowsVariance;

    public FixedList<DataType> TypeArguments { get; }

    private readonly Lazy<FixedSet<BareReferenceType>> supertypes;
    public FixedSet<BareReferenceType> Supertypes => supertypes.Value;

    public bool IsFullyKnown { [DebuggerStepThrough] get; }

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// const.
    /// </summary>
    public bool IsConstType => DeclaredType.IsConstType;

    private readonly Lazy<TypeReplacements> typeReplacements;

    protected BareReferenceType(DeclaredReferenceType declaredType, FixedList<DataType> typeArguments)
    {
        if (declaredType.GenericParameters.Count != typeArguments.Count)
            throw new ArgumentException(
                $"Number of type arguments must match. Given `[{typeArguments.ToILString()}]` for `{declaredType}`.",
                nameof(typeArguments));
        TypeArguments = typeArguments;
        IsFullyKnown = typeArguments.All(a => a.IsFullyKnown);
        typeReplacements = new(GetTypeReplacements);
        supertypes = new(GetSupertypes);
    }

    private TypeReplacements GetTypeReplacements() => new(DeclaredType, TypeArguments);

    private FixedSet<BareReferenceType> GetSupertypes()
        => DeclaredType.Supertypes.Select(typeReplacements.Value.ReplaceTypeParametersIn).ToFixedSet();

    public DataType ReplaceTypeParametersIn(DataType type)
        => typeReplacements.Value.ReplaceTypeParametersIn(type);

    public Pseudotype ReplaceTypeParametersIn(Pseudotype pseudotype)
        => typeReplacements.Value.ReplaceTypeParametersIn(pseudotype);

    public abstract BareReferenceType AccessedVia(ReferenceCapability capability);

    public abstract ReferenceType With(ReferenceCapability capability);

    #region Equality
    public abstract bool Equals(BareReferenceType? other);

    public abstract override int GetHashCode();

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((BareReferenceType)obj);
    }

    public static bool operator ==(BareReferenceType? left, BareReferenceType? right)
        => Equals(left, right);

    public static bool operator !=(BareReferenceType? left, BareReferenceType? right)
        => !Equals(left, right);
    #endregion

    [Obsolete("Use ToSourceCodeString() or ToILString() instead", error: true)]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    public sealed override string ToString()
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
        => throw new NotSupportedException();

    /// <summary>
    /// How this type would be written in source code.
    /// </summary>
    public abstract string ToSourceCodeString();

    /// <summary>
    /// How this type would be written in IL.
    /// </summary>
    public abstract string ToILString();
}
