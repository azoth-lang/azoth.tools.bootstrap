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
public abstract class BareType
{
    public static readonly BareReferenceType<DeclaredAnyType> Any = new(DeclaredType.Any, FixedList<DataType>.Empty);

    public abstract DeclaredType DeclaredType { get; }
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
    public bool IsDeclaredConstType => DeclaredType.IsConstType;

    private readonly Lazy<TypeReplacements> typeReplacements;

    public static BareReferenceType<DeclaredObjectType> Create(
        DeclaredObjectType declaredType,
        FixedList<DataType> typeArguments)
        => new(declaredType, typeArguments);

    private protected BareType(DeclaredType declaredType, FixedList<DataType> typeArguments)
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

    public abstract BareType With(FixedList<DataType> typeArguments);

    public abstract CapabilityType With(ReferenceCapability capability);

    public ObjectTypeConstraint With(ReferenceCapabilityConstraint capability)
        => new(capability, this);

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
