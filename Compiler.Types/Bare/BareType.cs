using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using MoreLinq.Extensions;

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

    public static readonly BareReferenceType<AnyType> Any = DeclaredType.Any.BareType;
    #endregion

    public abstract DeclaredType DeclaredType { get; }
    public IFixedList<DataType> GenericTypeArguments { get; }
    public IEnumerable<GenericParameterArgument> GenericParameterArguments
        => DeclaredType.GenericParameters.EquiZip(GenericTypeArguments, (p, a) => new GenericParameterArgument(p, a));
    public bool AllowsVariance { get; }
    public bool HasIndependentTypeArguments { get; }

    private readonly Lazy<IFixedSet<BareReferenceType>> supertypes;
    public IFixedSet<BareReferenceType> Supertypes => supertypes.Value;

    public bool IsFullyKnown { [DebuggerStepThrough] get; }

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// const.
    /// </summary>
    public bool IsDeclaredConstType => DeclaredType.IsDeclaredConst;

    private readonly Lazy<TypeReplacements> typeReplacements;

    public static BareReferenceType<ObjectType> Create(
        ObjectType declaredType,
        IFixedList<DataType> typeArguments)
        => new(declaredType, typeArguments);

    public static BareValueType<StructType> Create(
        StructType declaredType,
        IFixedList<DataType> typeArguments)
        => new(declaredType, typeArguments);

    private protected BareType(DeclaredType declaredType, IFixedList<DataType> genericTypeArguments)
    {
        if (declaredType.GenericParameters.Count != genericTypeArguments.Count)
            throw new ArgumentException(
                $"Number of type arguments must match. Given `[{genericTypeArguments.ToILString()}]` for `{declaredType}`.",
                nameof(genericTypeArguments));
        GenericTypeArguments = genericTypeArguments;
        AllowsVariance = declaredType.AllowsVariance
            || GenericTypeArguments.Any(a => a.AllowsVariance);
        HasIndependentTypeArguments = declaredType.HasIndependentGenericParameters
                                      || GenericTypeArguments.Any(a => a.HasIndependentTypeArguments);
        IsFullyKnown = genericTypeArguments.All(a => a.IsFullyKnown);

        typeReplacements = new(GetTypeReplacements);
        supertypes = new(GetSupertypes);
    }

    private TypeReplacements GetTypeReplacements() => new(DeclaredType, GenericTypeArguments);

    private IFixedSet<BareReferenceType> GetSupertypes()
        => DeclaredType.Supertypes.Select(typeReplacements.Value.ReplaceTypeParametersIn).ToFixedSet();

    public DataType ReplaceTypeParametersIn(DataType type)
        => typeReplacements.Value.ReplaceTypeParametersIn(type);

    public Pseudotype ReplaceTypeParametersIn(Pseudotype pseudotype)
        => typeReplacements.Value.ReplaceTypeParametersIn(pseudotype);

    public abstract BareType AccessedVia(Capability capability);

    protected IFixedList<DataType> TypeArgumentsAccessedVia(Capability capability)
    {
        var newTypeArguments = new List<DataType>(GenericTypeArguments.Count);
        var typesReplaced = false;
        foreach (var arg in GenericTypeArguments)
        {
            var newTypeArg = arg.AccessedVia(capability);
            typesReplaced |= !ReferenceEquals(newTypeArg, arg);
            newTypeArguments.Add(newTypeArg);
        }
        return typesReplaced ? newTypeArguments.ToFixedList() : GenericTypeArguments;
    }

    public abstract BareType With(IFixedList<DataType> typeArguments);

    public abstract CapabilityType With(Capability capability);

    public CapabilityTypeConstraint With(CapabilitySet capability)
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
        if (GenericTypeArguments.Any())
        {
            builder.Append('[');
            builder.AppendJoin(", ", GenericTypeArguments.Select(toString));
            builder.Append(']');
        }
        return builder.ToString();
    }
}
