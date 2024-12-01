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
    public PlainTypeReplacements(IOrdinaryTypeConstructor typeConstructor, IFixedList<IAntetype> typeArguments)
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
                var genericParameterPlainType = supertype.TypeConstructor.GenericParameterPlainTypes[i];
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

    public IMaybeExpressionAntetype ReplaceTypeParametersIn(IMaybeExpressionAntetype antetype)
        => antetype switch
        {
            IExpressionAntetype a => ReplaceTypeParametersIn(a),
            IMaybeAntetype a => ReplaceTypeParametersIn(a),
            _ => throw ExhaustiveMatch.Failed(antetype)
        };

    private IMaybeAntetype ReplaceTypeParametersIn(IMaybeAntetype antetype)
        => antetype switch
        {
            IAntetype a => ReplaceTypeParametersIn(a),
            UnknownAntetype a => a,
            _ => throw ExhaustiveMatch.Failed(antetype)
        };

    private IMaybeExpressionAntetype ReplaceTypeParametersIn(IExpressionAntetype antetype)
        => antetype switch
        {
            IAntetype a => ReplaceTypeParametersIn(a),
            LiteralTypeConstructor a => a,
            _ => throw ExhaustiveMatch.Failed(antetype)
        };

    private IAntetype ReplaceTypeParametersIn(IAntetype antetype)
        => antetype switch
        {
            VoidAntetype a => a,
            INonVoidAntetype a => ReplaceTypeParametersIn(a),
            _ => throw ExhaustiveMatch.Failed(antetype)
        };

    private IAntetype ReplaceTypeParametersIn(INonVoidAntetype antetype)
        => antetype switch
        {
            AnyAntetype a => a,
            SimpleTypeConstructor a => a,
            NeverAntetype a => a,
            SelfAntetype a => a,
            UserNonGenericNominalAntetype a => a,
            NamedPlainType a => ReplaceTypeParametersIn(a),
            GenericParameterPlainType a => ReplaceTypeParametersIn(a),
            FunctionAntetype a => ReplaceTypeParametersIn(a),
            OptionalAntetype a => ReplaceTypeParametersIn(a),
            _ => throw ExhaustiveMatch.Failed(antetype)
        };

    private NamedPlainType ReplaceTypeParametersIn(NamedPlainType antetype)
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

    private IAntetype ReplaceTypeParametersIn(GenericParameterPlainType plainType)
    {
        if (replacements.TryGetValue(plainType, out var replacementType))
            return replacementType;
        return plainType;
    }

    private FunctionAntetype ReplaceTypeParametersIn(FunctionAntetype antetype)
    {
        var replacementParameterTypes = ReplaceTypeParametersInParameters(antetype.Parameters);
        var replacementReturnType = ReplaceTypeParametersIn(antetype.Return);
        if (ReferenceEquals(antetype.Parameters, replacementParameterTypes)
            && ReferenceEquals(antetype.Return, replacementReturnType))
            return antetype;

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

    private IAntetype ReplaceTypeParametersIn(OptionalAntetype antetype)
    {
        var replacementType = ReplaceTypeParametersIn(antetype.Referent);
        if (ReferenceEquals(antetype.Referent, replacementType))
            return antetype;

        return replacementType.MakeOptional();
    }
}
