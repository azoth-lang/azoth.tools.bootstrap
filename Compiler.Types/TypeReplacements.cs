using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

internal sealed class TypeReplacements
{
    private readonly IDictionary<GenericParameterType, IType> replacements;

    /// <summary>
    /// Build a dictionary of type replacements. Generic parameter types of both this type and the
    /// supertypes can be replaced with type arguments of this type.
    /// </summary>
    public TypeReplacements(DeclaredType declaredType, IFixedList<IType> typeArguments)
    {
        replacements = declaredType.GenericParameterTypes.EquiZip(typeArguments)
                                   .ToDictionary(t => t.Item1, t => t.Item2);
        foreach (var supertype in declaredType.Supertypes)
            foreach (var (typeArg, i) in supertype.GenericTypeArguments.Enumerate())
            {
                var genericParameterType = supertype.DeclaredType.GenericParameterTypes[i];
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

    public IPseudotype ReplaceTypeParametersIn(IPseudotype pseudotype)
    {
        return pseudotype switch
        {
            IExpressionType type => ReplaceTypeParametersIn(type),
            CapabilityTypeConstraint type => ReplaceTypeParametersIn(type),
            _ => throw ExhaustiveMatch.Failed(pseudotype)
        };
    }

    public IMaybePseudotype ReplaceTypeParametersIn(IMaybePseudotype pseudotype)
    {
        return pseudotype switch
        {
            IMaybeExpressionType type => ReplaceTypeParametersIn(type),
            CapabilityTypeConstraint type => ReplaceTypeParametersIn(type),
            _ => throw ExhaustiveMatch.Failed(pseudotype)
        };
    }

    public CapabilityTypeConstraint ReplaceTypeParametersIn(CapabilityTypeConstraint pseudotype)
    {
        var replacementType = ReplaceTypeParametersIn(pseudotype.BareType);
        if (!ReferenceEquals(pseudotype.BareType, replacementType))
            return new CapabilityTypeConstraint(pseudotype.Capability, replacementType);
        return pseudotype;
    }

    public IMaybeExpressionType ReplaceTypeParametersIn(IMaybeExpressionType type)
    {
        return type switch
        {
            IExpressionType t => ReplaceTypeParametersIn(t),
            UnknownType _ => type,
            _ => throw ExhaustiveMatch.Failed(type)
        };
    }

    public IExpressionType ReplaceTypeParametersIn(IExpressionType type)
        => type switch
        {
            IType t => ReplaceTypeParametersIn(t),
            ConstValueType _ => type,
            _ => throw ExhaustiveMatch.Failed(type)
        };

    public IMaybeType ReplaceTypeParametersIn(IMaybeType type)
        => type switch
        {
            IType t => ReplaceTypeParametersIn(t),
            UnknownType _ => type,
            _ => throw ExhaustiveMatch.Failed(type)
        };

    public IType ReplaceTypeParametersIn(IType type)
    {
        switch (type)
        {
            case CapabilityType t:
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
                        return replacementType is INonVoidType nonVoidType
                            ? OptionalType.Create(nonVoidType)
                            // Optional of void is not allowed. Instead, just produce void.
                            : IType.Void;
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
                        return SelfViewpointType.Create(selfViewpointType.Capability, replacementType);
                    break;
                }
            case EmptyType _:
                break;
            default:
                throw ExhaustiveMatch.Failed(type);
        }

        return type;
    }

    public IType ReplaceTypeParametersIn(GenericParameterType type)
    {
        if (replacements.TryGetValue(type, out var replacementType))
            return replacementType;
        return type;
    }

    public BareNonVariableType ReplaceTypeParametersIn(BareNonVariableType type)
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

    public ParameterType? ReplaceTypeParametersIn(ParameterType type)
    {
        if (ReplaceTypeParametersIn(type.Type) is INonVoidType replacementType)
            return type with { Type = replacementType };
        return null;
    }

    private IFixedList<IType> ReplaceTypeParametersIn(IFixedList<IType> types)
    {
        var replacementTypes = new List<IType>();
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
}
