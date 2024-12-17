using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public sealed class BareTypeReplacements
{
    public static readonly BareTypeReplacements None = new();

    private readonly PlainTypeReplacements plainTypeReplacements;
    private readonly Dictionary<GenericParameterType, Type> replacements;

    private BareTypeReplacements()
    {
        plainTypeReplacements = PlainTypeReplacements.None;
        replacements = new();
    }

    /// <summary>
    /// Build a dictionary of type replacements. Generic parameter types of both this type and the
    /// containing type can be replaced with type arguments of this type.
    /// </summary>
    internal BareTypeReplacements(BareType bareSelfType)
    {
        plainTypeReplacements = bareSelfType.PlainType.TypeReplacements;
        replacements = bareSelfType.TypeConstructor.ParameterTypes.EquiZip(bareSelfType.Arguments)
                                   .ToDictionary(t => t.Item1, t => t.Item2);
    }

    public IMaybeType Apply(IMaybeType type)
        => type switch
        {
            Type t => Apply(t),
            UnknownType _ => type,
            _ => throw ExhaustiveMatch.Failed(type)
        };

    public Type Apply(Type type)
    {
        switch (type)
        {
            case CapabilityType t:
                return Apply(t);
            case OptionalType optionalType:
            {
                var replacementType = Apply(optionalType.Referent);
                if (!ReferenceEquals(optionalType.Referent, replacementType))
                    return replacementType is NonVoidType nonVoidType
                        ? OptionalType.Create(nonVoidType)
                        // Optional of void is not allowed. Instead, just produce void.
                        : Type.Void;
                break;
            }
            case GenericParameterType genericParameterType:
                return Apply(genericParameterType);
            case FunctionType functionType:
            {
                var replacementParameterTypes = Apply(functionType.Parameters);
                var replacementReturnType = Apply(functionType.Return);
                if (!ReferenceEquals(functionType.Parameters, replacementParameterTypes)
                    || !ReferenceEquals(functionType.Return, replacementReturnType))
                    return new FunctionType(replacementParameterTypes, replacementReturnType);
                break;
            }
            case CapabilityViewpointType capabilityViewpointType:
            {
                var replacementType = Apply(capabilityViewpointType.Referent);
                if (!ReferenceEquals(capabilityViewpointType.Referent, replacementType))
                    if (replacementType is GenericParameterType genericParameterType)
                        return CapabilityViewpointType.Create(capabilityViewpointType.Capability, genericParameterType);
                    else
                        return replacementType.AccessedVia(capabilityViewpointType.Capability);
                break;
            }
            case SelfViewpointType selfViewpointType:
            {
                var replacementType = Apply(selfViewpointType.Referent);
                if (!ReferenceEquals(selfViewpointType.Referent, replacementType))
                {
                    if (replacementType is NonVoidType nonVoidReplacementType)
                        return new SelfViewpointType(selfViewpointType.Capability, nonVoidReplacementType);
                    return Type.Void;
                }
                break;
            }
            case CapabilitySetSelfType _:
            case VoidType _:
            case NeverType _:
                break;
            default:
                throw ExhaustiveMatch.Failed(type);
        }

        return type;
    }

    public Type Apply(GenericParameterType type)
    {
        if (replacements.TryGetValue(type, out var replacementType))
            return replacementType;
        return type;
    }

    public Type Apply(CapabilityType type)
    {
        var replacementBareType = Apply(type.BareType);
        if (ReferenceEquals(type.BareType, replacementBareType)) return type;
        return replacementBareType.With(type.Capability);
    }

    public ParameterType? Apply(ParameterType type)
    {
        if (Apply(type.Type) is NonVoidType replacementType)
            return type with { Type = replacementType };
        return null;
    }

    public IMaybeParameterType? Apply(IMaybeParameterType type)
        => type switch
        {
            ParameterType t => Apply(t),
            UnknownType _ => Type.Unknown,
            _ => throw ExhaustiveMatch.Failed(type),
        };

    private IFixedList<Type> Apply(IFixedList<Type> types)
    {
        var replacementTypes = new List<Type>();
        var typesReplaced = false;
        foreach (var type in types)
        {
            var replacementType = Apply(type);
            typesReplaced |= !ReferenceEquals(type, replacementType);
            replacementTypes.Add(replacementType);
        }

        return typesReplaced ? replacementTypes.ToFixedList() : types;
    }

    private IFixedList<ParameterType> Apply(IFixedList<ParameterType> types)
    {
        var replacementTypes = new List<ParameterType>();
        var typesReplaced = false;
        foreach (var type in types)
        {
            var replacementType = Apply(type);
            if (replacementType is null)
                continue;
            typesReplaced |= !type.ReferenceEquals(replacementType);
            replacementTypes.Add(replacementType);
        }

        return typesReplaced ? replacementTypes.ToFixedList() : types;
    }

    [return: NotNullIfNotNull(nameof(bareType))]
    public BareType? Apply(BareType? bareType)
    {
        if (bareType is null) return null;
        var replacementContainingType = Apply(bareType.ContainingType);
        var replacementTypes = Apply(bareType.Arguments);
        if (ReferenceEquals(bareType.ContainingType, replacementContainingType)
            && ReferenceEquals(bareType.Arguments, replacementTypes)) return bareType;

        var plainType = plainTypeReplacements.Apply(bareType.PlainType);
        return new(plainType, replacementContainingType, replacementTypes);
    }
}
