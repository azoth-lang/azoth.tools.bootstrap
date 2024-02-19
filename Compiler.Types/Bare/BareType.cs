using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

/// <summary>
/// A type that could have a reference capability applied, but without a reference capability.
/// </summary>
[Closed(typeof(BareReferenceType), typeof(BareValueType))]
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public abstract class BareType : IEquatable<BareType>
{
    #region Standard Types
    public static readonly BareValueType<BoolType> Bool = DeclaredType.Bool.BareType;

    public static readonly BareValueType<BigIntegerType> Int = DeclaredType.Int.BareType;
    public static readonly BareValueType<BigIntegerType> UInt = DeclaredType.UInt.BareType;
    public static readonly BareValueType<FixedSizeIntegerType> Int8 = DeclaredType.Int8.BareType;
    public static readonly BareValueType<FixedSizeIntegerType> Byte = DeclaredType.Byte.BareType;
    public static readonly BareValueType<FixedSizeIntegerType> Int16 = DeclaredType.Int16.BareType;
    public static readonly BareValueType<FixedSizeIntegerType> UInt16 = DeclaredType.UInt16.BareType;
    public static readonly BareValueType<FixedSizeIntegerType> Int32 = DeclaredType.Int32.BareType;
    public static readonly BareValueType<FixedSizeIntegerType> UInt32 = DeclaredType.UInt32.BareType;
    public static readonly BareValueType<FixedSizeIntegerType> Int64 = DeclaredType.Int64.BareType;
    public static readonly BareValueType<FixedSizeIntegerType> UInt64 = DeclaredType.UInt64.BareType;
    public static readonly BareValueType<PointerSizedIntegerType> Size = new(DeclaredType.Size, FixedList.Empty<DataType>());
    public static readonly BareValueType<PointerSizedIntegerType> Offset = new(DeclaredType.Offset, FixedList.Empty<DataType>());

    public static readonly BareReferenceType<DeclaredAnyType> Any = DeclaredType.Any.BareType;
    #endregion

    public abstract DeclaredType DeclaredType { get; }
    public bool AllowsVariance => DeclaredType.AllowsVariance;
    public IFixedList<DataType> TypeArguments { get; }
    public bool HasIndependentTypeArguments { get; }

    private readonly Lazy<FixedSet<BareReferenceType>> supertypes;
    public FixedSet<BareReferenceType> Supertypes => supertypes.Value;

    public bool IsFullyKnown { [DebuggerStepThrough] get; }

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// const.
    /// </summary>
    public bool IsDeclaredConstType => DeclaredType.IsConstType;

    public TypeSemantics Semantics => DeclaredType.Semantics;

    private readonly Lazy<TypeReplacements> typeReplacements;

    public static BareReferenceType<DeclaredObjectType> Create(
        DeclaredObjectType declaredType,
        IFixedList<DataType> typeArguments)
        => new(declaredType, typeArguments);

    private protected BareType(DeclaredType declaredType, IFixedList<DataType> typeArguments)
    {
        if (declaredType.GenericParameters.Count != typeArguments.Count)
            throw new ArgumentException(
                $"Number of type arguments must match. Given `[{typeArguments.ToILString()}]` for `{declaredType}`.",
                nameof(typeArguments));
        TypeArguments = typeArguments;
        HasIndependentTypeArguments = declaredType.HasIndependentGenericParameters
                                      || TypeArguments.Any(a => a.HasIndependentTypeArguments);
        IsFullyKnown = typeArguments.All(a => a.IsFullyKnown);

        typeReplacements = new(GetTypeReplacements);
        supertypes = new(GetSupertypes);
    }

    private TypeReplacements GetTypeReplacements() => new(DeclaredType, TypeArguments);

    private FixedSet<BareReferenceType> GetSupertypes() =>
        DeclaredType.Supertypes.Select(typeReplacements.Value.ReplaceTypeParametersIn).ToFixedSet();

    public DataType ReplaceTypeParametersIn(DataType type)
        => typeReplacements.Value.ReplaceTypeParametersIn(type);

    public Pseudotype ReplaceTypeParametersIn(Pseudotype pseudotype)
        => typeReplacements.Value.ReplaceTypeParametersIn(pseudotype);

    public abstract BareType AccessedVia(ReferenceCapability capability);

    public abstract BareType With(IFixedList<DataType> typeArguments);

    public abstract CapabilityType With(ReferenceCapability capability);

    public ObjectTypeConstraint With(ReferenceCapabilityConstraint capability)
        => new(capability, this);

    #region Equality
    public sealed override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is BareType other && Equals(other);
    }

    public abstract bool Equals(BareType? other);

    public abstract override int GetHashCode();

    public static bool operator ==(BareType? left, BareType? right) => Equals(left, right);

    public static bool operator !=(BareType? left, BareType? right) => !Equals(left, right);
    #endregion

    public sealed override string ToString()
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
