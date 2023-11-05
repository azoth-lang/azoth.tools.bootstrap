using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
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

    public FixedList<DataType> TypeArguments { get; }

    public FixedSet<BareReferenceType> Supertypes { get; }

    public bool IsFullyKnown { [DebuggerStepThrough] get; }

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// const.
    /// </summary>
    public bool IsConstType => DeclaredType.IsConstType;


    private readonly FixedDictionary<DataType, DataType> typeReplacements;

    protected BareReferenceType(DeclaredReferenceType declaredType, FixedList<DataType> typeArguments)
    {
        if (declaredType.GenericParameters.Count != typeArguments.Count)
            throw new ArgumentException(
                $"Number of type arguments must match. Given `[{typeArguments.ToILString()}]` for `{declaredType}`.",
                nameof(typeArguments));
        TypeArguments = typeArguments;
        IsFullyKnown = typeArguments.All(a => a.IsFullyKnown);
        typeReplacements = declaredType.GenericParameterDataTypes.Zip(typeArguments).ToFixedDictionary();
        Supertypes = declaredType.Supertypes.Select(ReplaceTypeParametersIn).ToFixedSet();
    }

    public DataType ReplaceTypeParametersIn(DataType type)
    {
        if (typeReplacements.TryGetValue(type, out var replacementType)) return replacementType;
        switch (type)
        {
            case ObjectType objectType:
                return ReplaceTypeParametersIn(objectType);
            case OptionalType optionalType:
            {
                replacementType = ReplaceTypeParametersIn(optionalType.Referent);
                if (!ReferenceEquals(optionalType.Referent, replacementType)) return new OptionalType(replacementType);
                break;
            }
        }

        return type;
    }

    public BareReferenceType ReplaceTypeParametersIn(BareReferenceType type)
    {
        return type switch
        {
            BareObjectType objectType => ReplaceTypeParametersIn(objectType),
            BareAnyType _ => type,
            _ => throw ExhaustiveMatch.Failed(type)
        };
    }

    public ObjectType ReplaceTypeParametersIn(ObjectType type)
    {
        var replacementType = ReplaceTypeParametersIn(type.BareType);
        if (!ReferenceEquals(type.BareType, replacementType))
            return ObjectType.Create(type.Capability, replacementType);
        return type;
    }

    public BareObjectType ReplaceTypeParametersIn(BareObjectType type)
    {
        var replacementTypes = ReplaceTypeParametersIn(type.TypeArguments);
        if (!ReferenceEquals(type.TypeArguments, replacementTypes))
            return BareObjectType.Create(type.DeclaredType, replacementTypes);
        return type;
    }

    private FixedList<DataType> ReplaceTypeParametersIn(FixedList<DataType> types)
    {
        var replacementTypes = new List<DataType>();
        var typesReplaced = false;
        foreach (var type in types)
        {
            var replacementType = ReplaceTypeParametersIn(type);
            typesReplaced |= !ReferenceEquals(type, replacementType);
            replacementTypes.Add(replacementType);
        }

        return typesReplaced ? replacementTypes.ToFixedList() : types;
    }

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
