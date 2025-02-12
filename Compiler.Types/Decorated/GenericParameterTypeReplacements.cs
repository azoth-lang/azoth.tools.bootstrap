using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

/// <summary>
/// Replacements for the <see cref="GenericParameterType"/>s for a <see cref="BareType"/>.
/// </summary>
/// <remarks>This can also replace `self |>` and `Self` types.</remarks>
public sealed class GenericParameterTypeReplacements
{
    public static readonly GenericParameterTypeReplacements None = new();

    private readonly PlainTypeReplacements plainTypeReplacements;
    private readonly Dictionary<GenericParameterType, Type> replacements = new();

    private GenericParameterTypeReplacements()
    {
        plainTypeReplacements = PlainTypeReplacements.None;
    }

    /// <summary>
    /// Build a dictionary of type replacements. Generic parameter types of both this type and the
    /// containing type can be replaced with type arguments of this type.
    /// </summary>
    internal GenericParameterTypeReplacements(BareType bareType)
    {
        plainTypeReplacements = bareType.PlainType.TypeReplacements;
        AddReplacements(bareType);
    }

    private void AddReplacements(BareType bareType)
    {
        if (bareType.ContainingType is { } containingType)
            AddReplacements(containingType);
        var typeConstructor = bareType.TypeConstructor;
        foreach (var (parameter, arg) in typeConstructor.ParameterTypes.EquiZip(bareType.Arguments))
            replacements.Add(parameter, arg);

        // Set up replacements for supertype generic parameters
        // TODO this is needed because of the naive way that members are inherited without change. Instead,
        // they should be inherited with proper replacement and context. Then this can be removed.
        // Can't access bareType.Supertypes because that depends on the type replacements, use
        // `typeConstructor.Supertypes` instead.
        foreach (var supertype in typeConstructor.Supertypes)
        {
            foreach (var (parameter, arg) in supertype.TypeConstructor.ParameterTypes
                                                      .EquiZip(supertype.Arguments))
            {
                var replacement = ApplyTo(arg, null);
                if (!replacements.TryAdd(parameter, replacement)
                    && !replacements[parameter].Equals(replacement))
                    throw new NotImplementedException(
                        $"Conflicting type replacements. Replace `{parameter}` with "
                        + $"`{replacements[parameter]}` or `{replacement}`");
            }
        }
    }

    internal IMaybeType ApplyTo(IMaybeType type, NonVoidType? selfReplacement)
        => type switch
        {
            Type t => ApplyTo(t, selfReplacement),
            UnknownType _ => type,
            _ => throw ExhaustiveMatch.Failed(type)
        };

    internal Type ApplyTo(Type type, NonVoidType? selfReplacement)
    {
        switch (type)
        {
            case CapabilityType t:
                return ApplyTo(t, selfReplacement);
            case OptionalType t:
            {
                var replacementType = ApplyTo(t.Referent, selfReplacement);
                if (!ReferenceEquals(t.Referent, replacementType))
                    return OptionalType.CreateWithoutPlainType(replacementType);
                break;
            }
            case RefType t:
            {
                var replacementType = ApplyTo(t.Referent, selfReplacement);
                if (!ReferenceEquals(t.Referent, replacementType))
                    return RefType.CreateWithoutPlainType(t.IsInternal, t.IsMutableBinding, replacementType);
                break;
            }
            case GenericParameterType t:
                return ApplyTo(t);
            case FunctionType t:
            {
                var replacementParameterTypes = ApplyTo(t.Parameters, selfReplacement);
                var replacementReturnType = ApplyTo(t.Return, selfReplacement);
                if (!ReferenceEquals(t.Parameters, replacementParameterTypes)
                    || !ReferenceEquals(t.Return, replacementReturnType))
                    return new FunctionType(replacementParameterTypes, replacementReturnType);
                break;
            }
            case CapabilityViewpointType t:
            {
                var replacementType = ApplyTo(t.Referent);
                if (!ReferenceEquals(t.Referent, replacementType))
                    return replacementType.AccessedVia(t.Capability);
                break;
            }
            case SelfViewpointType t:
                return ApplyTo(t, selfReplacement);
            case CapabilitySetSelfType t:
                if (selfReplacement != null)
                    // A CapabilitySetSelfType can only occur as the `self` parameter to a method.
                    // As such, using the upper bound places the least restriction on what context
                    // the method can be called on.
                    return SelfReplacement(selfReplacement, t.CapabilitySet.UpperBound);
                break;
            case CapabilitySetRestrictedType t:
            {
                var replacementType = ApplyTo(t.Referent);
                if (!ReferenceEquals(t.Referent, replacementType))
                    switch (replacementType)
                    {
                        case CapabilityType replacement:
                        {
                            var capability = replacement.Capability.UpcastTo(t.CapabilitySet)
                                             ?? throw new NotImplementedException("Handle capability cannot be upcast to capability set.");
                            // The capability was already applied to the type, so the original
                            // capability accounted for whether the type was declared `const`. So there
                            // is no need to do anything for that now.
                            return replacement.BareType.With(capability);
                        }
                        case GenericParameterType replacement:
                            return CapabilitySetRestrictedType.Create(t.CapabilitySet, replacement);
                        default:
                            // TODO what is the correct thing to do in this case?
                            throw new NotImplementedException();
                    }
                break;
            }
            case VoidType _:
            case NeverType _:
                break;
            default:
                throw ExhaustiveMatch.Failed(type);
        }

        return type;
    }

    internal Type ApplyTo(SelfViewpointType type, NonVoidType? selfReplacement)
    {
        switch (selfReplacement)
        {
            case CapabilitySetSelfType r:
                return ApplyTo(type, r);
            case SelfViewpointType r:
                return ApplyTo(type, r);
            case CapabilityType r:
                return ApplyTo(type, r);
        }

        var replacementType = ApplyTo(type.Referent, selfReplacement);
        if (!ReferenceEquals(type.Referent, replacementType))
            return SelfViewpointType.Create(type.CapabilitySet, replacementType);

        return type;
    }

    internal Type ApplyTo(SelfViewpointType type, CapabilitySetSelfType selfReplacement)
    {
        var replacementType = ApplyTo(type.Referent, selfReplacement);
        // regardless of whether the replacement type changes, we need to apply the new capability
        return replacementType.AccessedVia(selfReplacement.CapabilitySet);
    }

    internal Type ApplyTo(SelfViewpointType type, SelfViewpointType selfReplacement)
    {
        var replacementType = ApplyTo(type.Referent, selfReplacement.Referent);
        // regardless of whether the replacement type changes, we need to apply the new capability
        return replacementType.AccessedVia(selfReplacement.CapabilitySet);
    }

    internal Type ApplyTo(SelfViewpointType type, CapabilityType selfReplacement)
    {
        var replacementType = ApplyTo(type.Referent, selfReplacement);
        // regardless of whether the replacement type changes, we need to apply the new capability
        return replacementType.AccessedVia(selfReplacement.Capability);
    }

    internal Type ApplyTo(GenericParameterType type)
    {
        if (replacements.TryGetValue(type, out var replacementType))
            return replacementType;
        return type;
    }

    internal Type ApplyTo(CapabilityType type, NonVoidType? selfReplacement)
    {
        if (selfReplacement != null && type is { TypeConstructor: SelfTypeConstructor })
            return SelfReplacement(selfReplacement, type.Capability);

        var replacementBareType = ApplyTo(type.BareType, selfReplacement);
        if (ReferenceEquals(type.BareType, replacementBareType)) return type;
        return replacementBareType.WithModified(type.Capability);
    }

    internal ParameterType? ApplyTo(ParameterType type, NonVoidType? selfReplacement)
    {
        if (ApplyTo(type.Type, selfReplacement) is NonVoidType replacementType)
            return type with { Type = replacementType };
        return null;
    }

    internal IMaybeParameterType? ApplyTo(IMaybeParameterType type, NonVoidType? selfReplacement)
        => type switch
        {
            ParameterType t => ApplyTo(t, selfReplacement),
            UnknownType _ => Type.Unknown,
            _ => throw ExhaustiveMatch.Failed(type),
        };

    private IFixedList<Type> ApplyTo(IFixedList<Type> types, NonVoidType? selfReplacement)
    {
        var replacementTypes = new List<Type>();
        var typesReplaced = false;
        foreach (var type in types)
        {
            var replacementType = ApplyTo(type, selfReplacement);
            typesReplaced |= !ReferenceEquals(type, replacementType);
            replacementTypes.Add(replacementType);
        }

        return typesReplaced ? replacementTypes.ToFixedList() : types;
    }

    private IFixedList<ParameterType> ApplyTo(IFixedList<ParameterType> types, NonVoidType? selfReplacement)
    {
        var replacementTypes = new List<ParameterType>();
        var typesReplaced = false;
        foreach (var type in types)
        {
            var replacementType = ApplyTo(type, selfReplacement);
            if (replacementType is null)
                continue;
            typesReplaced |= !type.ReferenceEquals(replacementType);
            replacementTypes.Add(replacementType);
        }

        return typesReplaced ? replacementTypes.ToFixedList() : types;
    }

    public BareType ApplyTo(BareType bareType)
        => ApplyTo(bareType, selfReplacement: null);

    [return: NotNullIfNotNull(nameof(bareType))]
    internal BareType? ApplyTo(BareType? bareType, NonVoidType? selfReplacement)
    {
        if (bareType is null) return null;

        var replacementContainingType = ApplyTo(bareType.ContainingType, selfReplacement);
        var replacementTypes = ApplyTo(bareType.Arguments, selfReplacement);
        if (ReferenceEquals(bareType.ContainingType, replacementContainingType)
            && ReferenceEquals(bareType.Arguments, replacementTypes)) return bareType;

        var plainType = plainTypeReplacements.ApplyTo(bareType.PlainType);
        return new(plainType, replacementContainingType, replacementTypes);
    }

    /// <summary>
    /// Produce the actual type to replace `Self` with. It should have the <paramref name="withCapability"/>.
    /// </summary>
    /// <remarks>The `Self` type is a bare type and doesn't carry any capabilities with it. So no
    /// capabilities are replaced. Instead, the capabilities in the type replacements are applied to
    /// is kept.</remarks>
    private static NonVoidType SelfReplacement(NonVoidType selfReplacement, Capability withCapability)
        => selfReplacement switch
        {
            CapabilitySetSelfType t => throw new NotSupportedException("Cannot replace `Self` with another `Self`."),
            CapabilityType t => t.BareType.WithModified(withCapability),
            CapabilityViewpointType t => CapabilityViewpointType.Create(withCapability, t.Referent),
            // TODO combine withCapability with the capability set somehow
            CapabilitySetRestrictedType t => throw new NotImplementedException(),
            FunctionType t => t,
            GenericParameterType t => t,
            NeverType t => t,
            OptionalType t => t,
            RefType t => t,
            SelfViewpointType t => SelfReplacement(t.Referent, withCapability),
            _ => throw ExhaustiveMatch.Failed(selfReplacement),
        };
}
