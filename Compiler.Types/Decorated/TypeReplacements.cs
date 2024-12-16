using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public sealed class TypeReplacements
{
    public static readonly TypeReplacements None = new();

    private readonly PlainTypeReplacements plainTypeReplacements;
    private readonly Dictionary<GenericParameterType, Type> replacements;

    private TypeReplacements()
    {
        plainTypeReplacements = PlainTypeReplacements.None;
        replacements = new();
    }

    /// <summary>
    /// Build a dictionary of type replacements. Generic parameter types of both this type and the
    /// supertypes can be replaced with type arguments of this type.
    /// </summary>
    internal TypeReplacements(
        PlainTypeReplacements plainTypeReplacements,
        TypeConstructor typeConstructor,
        IFixedList<Type> typeArguments)
    {
        this.plainTypeReplacements = plainTypeReplacements;
        replacements = typeConstructor.ParameterTypes.EquiZip(typeArguments)
                                   .ToDictionary(t => t.Item1, t => t.Item2);
        // Set up replacements for supertype generic parameters
        // TODO this might have been needed when inheritance was implemented by treating methods as
        //      if they were copied down the hierarchy, but I don't think it should be needed when
        //      they are properly handled.
        // TODO this also can't work because you can implement a trait multiple times with different type arguments
        //foreach (var supertype in typeConstructor.Supertypes)
        //{
        //    var genericParameterTypes = supertype.TypeConstructor.ParameterTypes;
        //    foreach (var (typeArg, i) in supertype.Arguments.Enumerate())
        //    {
        //        var genericParameterType = genericParameterTypes[i];
        //        if (typeArg is GenericParameterType genericTypeArg)
        //        {
        //            if (replacements.TryGetValue(genericTypeArg, out var replacement))
        //                replacements.Add(genericParameterType, replacement);
        //            else
        //                throw new InvalidOperationException(
        //                    $"Could not find replacement for `{typeArg.ToILString()}` in type `{typeConstructor}` with arguments `{typeArguments.ToILString()}`.");
        //        }
        //        else
        //            replacements.Add(genericParameterType, ReplaceTypeParametersIn(typeArg));
        //    }
        //}
    }

    // TODO shorten method names now that it is accessed view TypeReplacements.ReplaceTypeParametersIn

    public IMaybeType ReplaceTypeParametersIn(IMaybeType type)
        => type switch
        {
            Type t => ReplaceTypeParametersIn(t),
            UnknownType _ => type,
            _ => throw ExhaustiveMatch.Failed(type)
        };

    public Type ReplaceTypeParametersIn(Type type)
    {
        switch (type)
        {
            case CapabilityType t:
                return ReplaceTypeParametersIn(t);
            case OptionalType optionalType:
            {
                var replacementType = ReplaceTypeParametersIn(optionalType.Referent);
                if (!ReferenceEquals(optionalType.Referent, replacementType))
                    return replacementType is NonVoidType nonVoidType
                        ? OptionalType.Create(nonVoidType)
                        // Optional of void is not allowed. Instead, just produce void.
                        : Type.Void;
                break;
            }
            case GenericParameterType genericParameterType:
                return ReplaceTypeParametersIn(genericParameterType);
            case FunctionType functionType:
            {
                var replacementParameterTypes = ReplaceTypeParametersIn(functionType.Parameters);
                var replacementReturnType = ReplaceTypeParametersIn(functionType.Return);
                if (!ReferenceEquals(functionType.Parameters, replacementParameterTypes)
                    || !ReferenceEquals(functionType.Return, replacementReturnType))
                    return new FunctionType(replacementParameterTypes, replacementReturnType);
                break;
            }
            case CapabilityViewpointType capabilityViewpointType:
            {
                var replacementType = ReplaceTypeParametersIn(capabilityViewpointType.Referent);
                if (!ReferenceEquals(capabilityViewpointType.Referent, replacementType))
                    if (replacementType is GenericParameterType genericParameterType)
                        return CapabilityViewpointType.Create(capabilityViewpointType.Capability, genericParameterType);
                    else
                        return replacementType.AccessedVia(capabilityViewpointType.Capability);
                break;
            }
            case SelfViewpointType selfViewpointType:
            {
                var replacementType = ReplaceTypeParametersIn(selfViewpointType.Referent);
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

    public Type ReplaceTypeParametersIn(GenericParameterType type)
    {
        if (replacements.TryGetValue(type, out var replacementType))
            return replacementType;
        return type;
    }

    public Type ReplaceTypeParametersIn(CapabilityType type)
    {
        var replacementBareType = ReplaceTypeParametersIn(type.BareType);
        if (ReferenceEquals(type.BareType, replacementBareType)) return type;
        return replacementBareType.With(type.Capability);
    }

    public ParameterType? ReplaceTypeParametersIn(ParameterType type)
    {
        if (ReplaceTypeParametersIn(type.Type) is NonVoidType replacementType)
            return type with { Type = replacementType };
        return null;
    }

    public IMaybeParameterType? ReplaceTypeParametersIn(IMaybeParameterType type)
        => type switch
        {
            ParameterType t => ReplaceTypeParametersIn(t),
            UnknownType _ => Type.Unknown,
            _ => throw ExhaustiveMatch.Failed(type),
        };

    private IFixedList<Type> ReplaceTypeParametersIn(IFixedList<Type> types)
    {
        var replacementTypes = new List<Type>();
        var typesReplaced = false;
        foreach (var type in types)
        {
            var replacementType = ReplaceTypeParametersIn(type);
            typesReplaced |= !ReferenceEquals(type, replacementType);
            replacementTypes.Add(replacementType);
        }

        return typesReplaced ? replacementTypes.ToFixedList() : types;
    }

    private IFixedList<ParameterType> ReplaceTypeParametersIn(IFixedList<ParameterType> types)
    {
        var replacementTypes = new List<ParameterType>();
        var typesReplaced = false;
        foreach (var type in types)
        {
            var replacementType = ReplaceTypeParametersIn(type);
            if (replacementType is null)
                continue;
            typesReplaced |= !type.ReferenceEquals(replacementType);
            replacementTypes.Add(replacementType);
        }

        return typesReplaced ? replacementTypes.ToFixedList() : types;
    }

    public BareType ReplaceTypeParametersIn(BareType bareType)
    {
        var replacementTypes = ReplaceTypeParametersIn(bareType.Arguments);
        if (ReferenceEquals(bareType.Arguments, replacementTypes)) return bareType;

        var plainType = plainTypeReplacements.ReplaceTypeParametersIn(bareType.PlainType);
        return new(plainType, replacementTypes);
    }
}
