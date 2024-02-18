using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
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
    public TypeReplacements(DeclaredReferenceType declaredType, FixedList<DataType> typeArguments)
    {
        replacements = declaredType.GenericParameterTypes.Zip(typeArguments)
                                   .ToDictionary(t => t.First, t => t.Second);
        foreach (var supertype in declaredType.Supertypes)
            if (supertype is BareReferenceType referenceType)
                foreach (var (typeArg, i) in referenceType.TypeArguments.Enumerate())
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
            case ObjectType objectType:
            {
                var replacementType = ReplaceTypeParametersIn(objectType.BareType);
                if (!ReferenceEquals(objectType.BareType, replacementType))
                    return ObjectType.Create(objectType.Capability, replacementType);
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
                var replacementParameterTypes = ReplaceTypeParametersIn(functionType.ParameterTypes);
                var replacementReturnType = ReplaceTypeParametersIn(functionType.ReturnType);
                if (!ReferenceEquals(functionType.ParameterTypes, replacementParameterTypes)
                    || !functionType.ReturnType.ReferenceEquals(replacementReturnType))
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
            case SimpleType _:
            case EmptyType _:
            case UnknownType _:
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

    public BareReferenceType ReplaceTypeParametersIn(BareReferenceType type)
    {
        var replacementTypes = ReplaceTypeParametersIn(type.TypeArguments);
        if (ReferenceEquals(type.TypeArguments, replacementTypes))
            return type;

        return BareReferenceType.Create(type.DeclaredType, replacementTypes);
    }

    public ParameterType ReplaceTypeParametersIn(ParameterType type)
        => type with { Type = ReplaceTypeParametersIn(type.Type) };

    public ReturnType ReplaceTypeParametersIn(ReturnType returnType)
        => returnType with { Type = ReplaceTypeParametersIn(returnType.Type) };

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

    private FixedList<ParameterType> ReplaceTypeParametersIn(FixedList<ParameterType> types)
    {
        var replacementTypes = new List<ParameterType>();
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
