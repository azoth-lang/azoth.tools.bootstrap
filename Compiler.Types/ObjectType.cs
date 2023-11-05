using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// Object types are the types created with class and trait declarations. An
/// object type may have generic parameters that may be filled with generic
/// arguments. An object type with generic parameters but no generic arguments
/// is an *unbound type*. One with generic arguments supplied for all
/// parameters is *a constructed type*. One with some but not all arguments
/// supplied is *partially constructed type*.
/// </summary>
/// <remarks>
/// There will be two special object types `Type` and `Metatype`
/// </remarks>
public sealed class ObjectType : ReferenceType
{
    public new DeclaredObjectType DeclaredType => (DeclaredObjectType)base.DeclaredType;

    public FixedList<DataType> TypeArguments { get; }
    public override bool IsFullyKnown { [DebuggerStepThrough] get; }

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// const.
    /// </summary>
    public bool IsConst => DeclaredType.IsConst;

    private readonly FixedDictionary<DataType, DataType> typeReplacements;

    /// <summary>
    /// Create a object type for a given class or trait.
    /// </summary>
    public static ObjectType Create(
        ReferenceCapability capability,
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        bool isClass,
        string name)
        => new(capability, DeclaredObjectType.Create(containingPackage, containingNamespace,
            isAbstract, isConst, isClass, name), FixedList<DataType>.Empty);

    /// <summary>
    /// Create a object type for a given class or trait.
    /// </summary>
    public static ObjectType Create(
        ReferenceCapability capability,
        DeclaredObjectType declaredType,
        FixedList<DataType> typeArguments)
        => new(capability, declaredType, typeArguments);

    private ObjectType(
        ReferenceCapability capability,
        DeclaredObjectType declaredType,
        FixedList<DataType> typeArguments)
        : base(capability, declaredType)
    {
        if (declaredType.GenericParameters.Count != typeArguments.Count)
            throw new ArgumentException($"Number of type arguments must match. Given `[{typeArguments.ToILString()}]` for `{declaredType}`.", nameof(typeArguments));
        TypeArguments = typeArguments;
        IsFullyKnown = typeArguments.All(a => a.IsFullyKnown);
        typeReplacements = declaredType.GenericParameterDataTypes.Zip(typeArguments).ToFixedDictionary();
    }

    public override ObjectType With(ReferenceCapability referenceCapability)
        => new(referenceCapability, DeclaredType, TypeArguments);

    /// <remarks>For constant types, there can still be read only references. For example, inside
    /// the constructor.</remarks>
    public override ObjectType WithoutWrite() => (ObjectType)base.WithoutWrite();

    public override DataType ReplaceTypeParametersIn(DataType type)
    {
        if (typeReplacements.TryGetValue(type, out var replacementType))
            return replacementType;
        switch (type)
        {
            case ObjectType objectType:
            {
                var replacementTypes = ReplaceTypeParametersIn(objectType.TypeArguments);
                if (!ReferenceEquals(objectType.TypeArguments, replacementTypes))
                    return new ObjectType(objectType.Capability, objectType.DeclaredType, replacementTypes);
                break;
            }
            case OptionalType optionalType:
            {
                replacementType = ReplaceTypeParametersIn(optionalType.Referent);
                if (!ReferenceEquals(optionalType.Referent, replacementType))
                    return new OptionalType(replacementType);
                break;
            }
        }
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
    public override bool Equals(DataType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ObjectType otherType
               && Capability == otherType.Capability
               && DeclaredType == otherType.DeclaredType
               && TypeArguments.Equals(otherType.TypeArguments);
    }

    public override int GetHashCode() => HashCode.Combine(DeclaredType, Capability, TypeArguments);
    #endregion

    public override string ToSourceCodeString()
    {
        var builder = new StringBuilder();
        if (Capability != ReferenceCapability.ReadOnly)
        {
            builder.Append(Capability.ToSourceString());
            builder.Append(' ');
        }
        AppendName(builder, t => t.ToSourceCodeString());
        return builder.ToString();
    }

    public override string ToILString()
    {
        var builder = new StringBuilder();
        builder.Append(Capability.ToILString());
        builder.Append(' ');
        AppendName(builder, t => t.ToILString());
        return builder.ToString();
    }

    private void AppendName(StringBuilder builder, Func<DataType, string> toString)
    {
        builder.Append(ContainingNamespace);
        if (ContainingNamespace != NamespaceName.Global) builder.Append('.');
        builder.Append(Name.ToBareString());
        if (!TypeArguments.Any()) return;
        builder.Append('[');
        builder.AppendJoin(", ", TypeArguments.Select(toString));
        builder.Append(']');
    }
}
