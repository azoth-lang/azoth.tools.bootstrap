using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

internal sealed class PlainTypeReplacements
{
    private readonly Dictionary<GenericParameterPlainType, IAntetype> replacements;

    /// <summary>
    /// Build a dictionary of type replacements. Generic parameter types of both this type and the
    /// supertypes can be replaced with type arguments of this type.
    /// </summary>
    public PlainTypeReplacements(ITypeConstructor typeConstructor, IFixedList<IAntetype> typeArguments)
    {
        replacements = typeConstructor.GenericParameterPlainTypes.EquiZip(typeArguments)
                                   .ToDictionary(t => t.Item1, t => t.Item2);
        if (typeConstructor.Parameters.IsEmpty)
            return;
        // Set up replacements for supertype generic parameters
        // TODO this might have been needed when inheritance was implemented by treating methods as
        //      if they were copied down the hierarchy, but I don't think it should be needed when
        //      they are properly handled.
        foreach (var supertype in typeConstructor.Supertypes)
            foreach (var (supertypeArgument, i) in supertype.TypeArguments.Enumerate())
            {
                var genericParameterPlainType = supertype.TypeConstructor?.GenericParameterPlainTypes[i];
                if (genericParameterPlainType is null)
                    continue;
                if (supertypeArgument is GenericParameterPlainType genericAntetypeArg)
                {
                    if (replacements.TryGetValue(genericAntetypeArg, out var replacement))
                        replacements.Add(genericParameterPlainType, replacement);
                    else
                        throw new InvalidOperationException(
                            $"Could not find replacement for `{supertypeArgument}` in type constructor `{typeConstructor}` with arguments `{typeArguments}`.");
                }
                else
                    replacements.Add(genericParameterPlainType, ReplaceTypeParametersIn(supertypeArgument));
            }
    }

    public IMaybeAntetype ReplaceTypeParametersIn(IMaybeAntetype antetype)
        => antetype switch
        {
            IAntetype a => ReplaceTypeParametersIn(a),
            UnknownPlainType a => a,
            _ => throw ExhaustiveMatch.Failed(antetype)
        };

    public IAntetype ReplaceTypeParametersIn(IAntetype antetype)
        => antetype switch
        {
            VoidPlainType a => a,
            LiteralTypeConstructor a => a,
            INonVoidAntetype a => ReplaceTypeParametersIn(a),
            _ => throw ExhaustiveMatch.Failed(antetype)
        };

    public IAntetype ReplaceTypeParametersIn(INonVoidAntetype antetype)
        => antetype switch
        {
            SimpleTypeConstructor a => a,
            NeverPlainType a => a,
            SelfPlainType a => a,
            OrdinaryNamedPlainType a => ReplaceTypeParametersIn(a),
            GenericParameterPlainType a => ReplaceTypeParametersIn(a),
            FunctionPlainType a => ReplaceTypeParametersIn(a),
            OptionalPlainType a => ReplaceTypeParametersIn(a),
            _ => throw ExhaustiveMatch.Failed(antetype)
        };

    public OrdinaryNamedPlainType ReplaceTypeParametersIn(OrdinaryNamedPlainType antetype)
    {
        var replacementTypeArguments = ReplaceTypeParametersIn(antetype.TypeArguments);
        if (ReferenceEquals(antetype.TypeArguments, replacementTypeArguments))
            return antetype;

        return new(antetype.TypeConstructor, replacementTypeArguments);
    }

    private IFixedList<IAntetype> ReplaceTypeParametersIn(IFixedList<IAntetype> antetypes)
    {
        var replacementAntetypes = new List<IAntetype>();
        var typesReplaced = false;
        foreach (var antetype in antetypes)
        {
            var replacementType = ReplaceTypeParametersIn(antetype);
            typesReplaced |= !ReferenceEquals(antetype, replacementType);
            replacementAntetypes.Add(replacementType);
        }

        return typesReplaced ? replacementAntetypes.ToFixedList() : antetypes;
    }

    public IAntetype ReplaceTypeParametersIn(GenericParameterPlainType plainType)
        => replacements.GetValueOrDefault(plainType, plainType);

    public FunctionPlainType ReplaceTypeParametersIn(FunctionPlainType plainType)
    {
        var replacementParameterTypes = ReplaceTypeParametersInParameters(plainType.Parameters);
        var replacementReturnType = ReplaceTypeParametersIn(plainType.Return);
        if (ReferenceEquals(plainType.Parameters, replacementParameterTypes)
            && ReferenceEquals(plainType.Return, replacementReturnType))
            return plainType;

        return new(replacementParameterTypes, replacementReturnType);
    }

    /// <summary>
    /// Replace type parameters specifically in parameters to a function where `void` can cause
    /// a parameter to drop out.
    /// </summary>
    private IFixedList<INonVoidAntetype> ReplaceTypeParametersInParameters(
        IFixedList<INonVoidAntetype> antetypes)
    {
        var replacementAntetypes = new List<INonVoidAntetype>();
        var typesReplaced = false;
        foreach (var antetype in antetypes)
        {
            var replacementType = ReplaceTypeParametersIn(antetype);
            typesReplaced |= !ReferenceEquals(antetype, replacementType);
            if (replacementType is INonVoidAntetype nonVoidAntetype)
                replacementAntetypes.Add(nonVoidAntetype);
        }

        return typesReplaced ? replacementAntetypes.ToFixedList() : antetypes;
    }

    public IAntetype ReplaceTypeParametersIn(OptionalPlainType plainType)
    {
        var replacementType = ReplaceTypeParametersIn(plainType.Referent);
        if (ReferenceEquals(plainType.Referent, replacementType))
            return plainType;

        return replacementType.MakeOptional();
    }
}
