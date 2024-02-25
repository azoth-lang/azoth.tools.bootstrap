using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

internal sealed class TypeReplacements
{
    private readonly IDictionary<GenericParameterType, DataType> replacements;

    /// <summary>
    /// Build a dictionary of type replacements. Generic parameter types of both this type and the
    /// supertypes can be replaced with type arguments of this type.
    /// </summary>
    public TypeReplacements(DeclaredType declaredType, IFixedList<DataType> typeArguments)
    {
        replacements = declaredType.GenericParameterTypes.Zip(typeArguments)
                                   .ToDictionary(t => t.First, t => t.Second);
        foreach (var supertype in declaredType.Supertypes)
            if (supertype is BareReferenceType referenceType)
                foreach (var (typeArg, i) in referenceType.GenericTypeArguments.Enumerate())
                {
                    var genericParameterType = referenceType.DeclaredType.GenericParameterTypes[i];
                    if (typeArg is GenericParameterType genericTypeArg)
                    {
                        if (replacements.TryGetValue(genericTypeArg, out var replacement))
                            replacements.Add(genericParameterType, replacement);
                        else
                            throw new InvalidOperationException(
                                $"Could not find replacement for `{typeArg}` in type `{declaredType}` with arguments `{typeArguments}`.");
                    }
                    else
                        replacements.Add(genericParameterType, ReplaceTypeParametersIn(typeArg));
                }
    }

    public Pseudotype ReplaceTypeParametersIn(Pseudotype pseudotype)
    {
        return pseudotype switch
        {
            DataType type => ReplaceTypeParametersIn(type),
            ObjectTypeConstraint type => ReplaceTypeParametersIn(type),
            _ => throw ExhaustiveMatch.Failed(pseudotype)
        };
    }

    public ObjectTypeConstraint ReplaceTypeParametersIn(ObjectTypeConstraint pseudotype)
    {
        var replacementType = ReplaceTypeParametersIn(pseudotype.BareType);
        if (!ReferenceEquals(pseudotype.BareType, replacementType))
            return new ObjectTypeConstraint(pseudotype.Capability, replacementType);
        return pseudotype;
    }

    public DataType ReplaceTypeParametersIn(DataType type)
    {
        switch (type)
        {
            case ValueType t:
            {
                var replacementType = ReplaceTypeParametersIn(t.BareType);
                if (!ReferenceEquals(t.BareType, replacementType))
                    // TODO use proper capability
                    return replacementType.With(Capability.Constant);
                break;
            }
            case ReferenceType t:
            {
                var replacementType = ReplaceTypeParametersIn(t.BareType);
                if (!ReferenceEquals(t.BareType, replacementType))
                    return replacementType.With(t.Capability);
                break;
            }
            case OptionalType optionalType:
            {
                var replacementType = ReplaceTypeParametersIn(optionalType.Referent);
                if (!ReferenceEquals(optionalType.Referent, replacementType))
                    return new OptionalType(replacementType);
                break;
            }
            case GenericParameterType genericParameterType:
                return ReplaceTypeParametersIn(genericParameterType);
            case FunctionType functionType:
            {
                var replacementParameterTypes = ReplaceTypeParametersIn(functionType.Parameters);
                var replacementReturnType = ReplaceTypeParametersIn(functionType.Return);
                if (!ReferenceEquals(functionType.Parameters, replacementParameterTypes)
                    || !functionType.Return.ReferenceEquals(replacementReturnType))
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
                    return new SelfViewpointType(selfViewpointType.Capability, replacementType);
                break;
            }
            case EmptyType _:
            case UnknownType _:
            case ConstValueType _:
                break;
            default:
                throw ExhaustiveMatch.Failed(type);
        }

        return type;
    }

    public DataType ReplaceTypeParametersIn(GenericParameterType type)
    {
        if (replacements.TryGetValue(type, out var replacementType))
            return replacementType;
        return type;
    }

    public BareType ReplaceTypeParametersIn(BareType type)
    {
        var replacementTypes = ReplaceTypeParametersIn(type.GenericTypeArguments);
        if (ReferenceEquals(type.GenericTypeArguments, replacementTypes)) return type;

        return type.With(replacementTypes);
    }

    public BareReferenceType ReplaceTypeParametersIn(BareReferenceType type)
    {
        var replacementTypes = ReplaceTypeParametersIn(type.GenericTypeArguments);
        if (ReferenceEquals(type.GenericTypeArguments, replacementTypes))
            return type;

        return type.With(replacementTypes);
    }

    public Parameter ReplaceTypeParametersIn(Parameter type)
        => type with { Type = ReplaceTypeParametersIn(type.Type) };

    public Return ReplaceTypeParametersIn(Return @return)
        => @return with { Type = ReplaceTypeParametersIn(@return.Type) };

    private IFixedList<DataType> ReplaceTypeParametersIn(IFixedList<DataType> types)
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

    private IFixedList<Parameter> ReplaceTypeParametersIn(IFixedList<Parameter> types)
    {
        var replacementTypes = new List<Parameter>();
        var typesReplaced = false;
        foreach (var type in types)
        {
            var replacementType = ReplaceTypeParametersIn(type);
            typesReplaced |= !type.ReferenceEquals(replacementType);
            replacementTypes.Add(replacementType);
        }

        return typesReplaced ? replacementTypes.ToFixedList() : types;
    }
}
