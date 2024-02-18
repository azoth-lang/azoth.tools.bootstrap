using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

/// <summary>
/// An reference type without a reference capability.
/// </summary>
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public abstract class BareReferenceType : IEquatable<BareReferenceType>
{
    public static readonly BareReferenceType<DeclaredAnyType> Any = new(DeclaredReferenceType.Any, FixedList<DataType>.Empty);

    public bool AllowsVariance => DeclaredType.AllowsVariance;

    public FixedList<DataType> TypeArguments { get; }

    public bool HasIndependentTypeArguments { get; }

    private readonly Lazy<FixedSet<BareReferenceType>> supertypes;
    public FixedSet<BareReferenceType> Supertypes => supertypes.Value;

    public bool IsFullyKnown { [DebuggerStepThrough] get; }

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// const.
    /// </summary>
    public bool IsConstType => DeclaredType.IsConstType;

    public abstract DeclaredReferenceType DeclaredType { get; }

    private readonly Lazy<TypeReplacements> typeReplacements;

    public static BareReferenceType<DeclaredObjectType> Create(DeclaredObjectType declaredType, FixedList<DataType> typeArguments)
        => new(declaredType, typeArguments);

    protected BareReferenceType(DeclaredReferenceType declaredType, FixedList<DataType> typeArguments)
    {
        if (declaredType.GenericParameters.Count != typeArguments.Count)
            throw new ArgumentException(
                $"Number of type arguments must match. Given `[{typeArguments.ToILString()}]` for `{declaredType}`.",
                nameof(typeArguments));
        TypeArguments = typeArguments;
        HasIndependentTypeArguments = declaredType.HasIndependentGenericParameters || TypeArguments.Any(a => a.HasIndependentTypeArguments);
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

    public abstract BareReferenceType With(FixedList<DataType> typeArguments);

    public abstract ReferenceType With(ReferenceCapability capability);

    public ObjectTypeConstraint With(ReferenceCapabilityConstraint capability)
        => new(capability, this);

    #region Equality
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is BareReferenceType other && Equals(other);
    }

    public virtual bool Equals(BareReferenceType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is BareReferenceType otherType
               && DeclaredType == otherType.DeclaredType
               && TypeArguments.Equals(otherType.TypeArguments);
    }

    public override int GetHashCode() => HashCode.Combine(DeclaredType, TypeArguments);

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

    public virtual string ToSourceCodeString() => ToString(t => t.ToSourceCodeString());
    public virtual string ToILString() => ToString(t => t.ToILString());

    private string ToString(Func<DataType, string> toString)
    {
        var builder = new StringBuilder();
        builder.Append(DeclaredType.ContainingNamespace);
        if (DeclaredType.ContainingNamespace != NamespaceName.Global) builder.Append('.');
        builder.Append(DeclaredType.Name.ToBareString());
        if (TypeArguments.Any())
        {
            builder.Append('[');
            builder.AppendJoin(", ", TypeArguments.Select(toString));
            builder.Append(']');
        }
        return builder.ToString();
    }
}

public sealed class BareReferenceType<TDeclared> : BareReferenceType
    where TDeclared : DeclaredReferenceType
{
    public override TDeclared DeclaredType { get; }

    internal BareReferenceType(TDeclared declaredType, FixedList<DataType> typeArguments)
        : base(declaredType, typeArguments)
    {
        DeclaredType = declaredType;
    }

    public override BareReferenceType<TDeclared> AccessedVia(ReferenceCapability capability)
    {
        if (DeclaredType.GenericParameters.All(p => p.Variance != Variance.Independent)) return this;
        var newTypeArguments = DeclaredType.GenericParameters.Zip(TypeArguments,
            (p, arg) => p.Variance == Variance.Independent ? arg.AccessedVia(capability) : arg).ToFixedList();
        return new(DeclaredType, newTypeArguments);
    }

    public override BareReferenceType<TDeclared> With(FixedList<DataType> typeArguments)
        => new(DeclaredType, typeArguments);

    public override ReferenceType<TDeclared> With(ReferenceCapability capability)
        => new ReferenceType<TDeclared>(capability, this);
}
