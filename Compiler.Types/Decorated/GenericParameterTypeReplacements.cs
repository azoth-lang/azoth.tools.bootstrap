using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

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
                var replacement = Apply(arg, null);
                if (!replacements.TryAdd(parameter, replacement)
                    && !replacements[parameter].Equals(replacement))
                    throw new NotImplementedException(
                        $"Conflicting type replacements. Replace `{parameter}` with "
                        + $"`{replacements[parameter]}` or `{replacement}`");
            }
        }
    }

    internal IMaybeType Apply(IMaybeType type, NonVoidType? selfReplacement)
        => type switch
        {
            Type t => Apply(t, selfReplacement),
            UnknownType _ => type,
            _ => throw ExhaustiveMatch.Failed(type)
        };

    internal Type Apply(Type type, NonVoidType? selfReplacement)
    {
        switch (type)
        {
            case CapabilityType t:
                return Apply(t, selfReplacement);
            case OptionalType t:
            {
                var replacementType = Apply(t.Referent, selfReplacement);
                if (!ReferenceEquals(t.Referent, replacementType))
                    return OptionalType.CreateWithoutPlainType(replacementType);
                break;
            }
            case RefType t:
            {
                var replacementType = Apply(t.Referent, selfReplacement);
                if (!ReferenceEquals(t.Referent, replacementType))
                    return RefType.CreateWithoutPlainType(t.IsInternal, t.IsMutableBinding, replacementType);
                break;
            }
            case GenericParameterType genericParameterType:
                return Apply(genericParameterType);
            case FunctionType functionType:
            {
                var replacementParameterTypes = Apply(functionType.Parameters, selfReplacement);
                var replacementReturnType = Apply(functionType.Return, selfReplacement);
                if (!ReferenceEquals(functionType.Parameters, replacementParameterTypes)
                    || !ReferenceEquals(functionType.Return, replacementReturnType))
                    return new FunctionType(replacementParameterTypes, replacementReturnType);
                break;
            }
            case CapabilityViewpointType capabilityViewpointType:
            {
                var replacementType = Apply(capabilityViewpointType.Referent);
                if (!ReferenceEquals(capabilityViewpointType.Referent, replacementType))
                    return replacementType.AccessedVia(capabilityViewpointType.Capability);
                break;
            }
            case SelfViewpointType selfViewpointType:
                return Apply(selfViewpointType, selfReplacement);
            case CapabilitySetSelfType t:
            {
                if (selfReplacement is not null)
                {
                    if (selfReplacement is CapabilityType { Capability: var selfCapability })
                        Requires.That(t.CapabilitySet.AllowedCapabilities.Contains(selfCapability), nameof(selfReplacement),
                            "Must have a compatible capability.");
                    return selfReplacement;
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

    internal Type Apply(SelfViewpointType type, NonVoidType? selfReplacement)
    {
        // TODO what about CapabilitySetTypes and other types?
        if (selfReplacement is CapabilityType capabilitySelfReplacement)
            return Apply(type, capabilitySelfReplacement);

        var replacementType = Apply(type.Referent, selfReplacement);
        if (!ReferenceEquals(type.Referent, replacementType))
            return SelfViewpointType.Create(type.CapabilitySet, replacementType);

        return type;
    }

    internal Type Apply(SelfViewpointType type, CapabilityType selfReplacement)
    {
        var replacementType = Apply(type.Referent, selfReplacement);
        // regardless of whether the replacement type changes, we need to apply the new capability
        return replacementType.AccessedVia(selfReplacement.Capability);
    }

    internal Type Apply(GenericParameterType type)
    {
        if (replacements.TryGetValue(type, out var replacementType))
            return replacementType;
        return type;
    }

    internal Type Apply(CapabilityType type, NonVoidType? selfReplacement)
    {
        // TODO what about CapabilitySetTypes and other types?
        var bareSelfReplacement = (selfReplacement as CapabilityType)?.BareType;
        var replacementBareType = Apply(type.BareType, selfReplacement, bareSelfReplacement);
        if (ReferenceEquals(type.BareType, replacementBareType)) return type;
        return replacementBareType.WithModified(type.Capability);
    }

    internal ParameterType? Apply(ParameterType type, NonVoidType? selfReplacement)
    {
        if (Apply(type.Type, selfReplacement) is NonVoidType replacementType)
            return type with { Type = replacementType };
        return null;
    }

    internal IMaybeParameterType? Apply(IMaybeParameterType type, NonVoidType? selfReplacement)
        => type switch
        {
            ParameterType t => Apply(t, selfReplacement),
            UnknownType _ => Type.Unknown,
            _ => throw ExhaustiveMatch.Failed(type),
        };

    private IFixedList<Type> Apply(IFixedList<Type> types, NonVoidType? selfReplacement)
    {
        var replacementTypes = new List<Type>();
        var typesReplaced = false;
        foreach (var type in types)
        {
            var replacementType = Apply(type, selfReplacement);
            typesReplaced |= !ReferenceEquals(type, replacementType);
            replacementTypes.Add(replacementType);
        }

        return typesReplaced ? replacementTypes.ToFixedList() : types;
    }

    private IFixedList<ParameterType> Apply(IFixedList<ParameterType> types, NonVoidType? selfReplacement)
    {
        var replacementTypes = new List<ParameterType>();
        var typesReplaced = false;
        foreach (var type in types)
        {
            var replacementType = Apply(type, selfReplacement);
            if (replacementType is null)
                continue;
            typesReplaced |= !type.ReferenceEquals(replacementType);
            replacementTypes.Add(replacementType);
        }

        return typesReplaced ? replacementTypes.ToFixedList() : types;
    }

    public BareType Apply(BareType bareType)
        => Apply(bareType, selfReplacement: null, bareSelfReplacement: null);

    [return: NotNullIfNotNull(nameof(bareType))]
    internal BareType? Apply(BareType? bareType, NonVoidType? selfReplacement, BareType? bareSelfReplacement)
    {
        if (bareType is null) return null;

        if (bareSelfReplacement is not null && bareType is { TypeConstructor: SelfTypeConstructor })
            // TODO doesn't the replacement type need to match the Self context type?
            return bareSelfReplacement;

        var replacementContainingType = Apply(bareType.ContainingType, selfReplacement, bareSelfReplacement);
        var replacementTypes = Apply(bareType.Arguments, selfReplacement);
        if (ReferenceEquals(bareType.ContainingType, replacementContainingType)
            && ReferenceEquals(bareType.Arguments, replacementTypes)) return bareType;

        var plainType = plainTypeReplacements.Apply(bareType.PlainType);
        return new(plainType, replacementContainingType, replacementTypes);
    }
}
