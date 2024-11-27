using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain.ConstValue;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

internal sealed class AntetypeReplacements
{
    private readonly IDictionary<GenericParameterAntetype, IAntetype> replacements;

    /// <summary>
    /// Build a dictionary of type replacements. Generic parameter types of both this type and the
    /// supertypes can be replaced with type arguments of this type.
    /// </summary>
    public AntetypeReplacements(IUserDeclaredAntetype declaredType, IFixedList<IAntetype> typeArguments)
    {
        replacements = declaredType.GenericParameterAntetypes.EquiZip(typeArguments)
                                   .ToDictionary(t => t.Item1, t => t.Item2);
        foreach (var supertype in declaredType.Supertypes)
            foreach (var (antetypeArg, i) in supertype.TypeArguments.Enumerate())
            {
                var genericParameterAnteype = supertype.DeclaredAntetype.GenericParameterAntetypes[i];
                if (antetypeArg is GenericParameterAntetype genericAntetypeArg)
                {
                    if (replacements.TryGetValue(genericAntetypeArg, out var replacement))
                        replacements.Add(genericParameterAnteype, replacement);
                    else
                        throw new InvalidOperationException(
                            $"Could not find replacement for `{antetypeArg}` in antetype `{declaredType}` with arguments `{typeArguments}`.");
                }
                else
                    replacements.Add(genericParameterAnteype, ReplaceTypeParametersIn(antetypeArg));
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
            ConstValueAntetype a => a,
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
            SimpleAntetype a => a,
            NeverAntetype a => a,
            SelfAntetype a => a,
            UserNonGenericNominalAntetype a => a,
            UserGenericNominalAntetype a => ReplaceTypeParametersIn(a),
            GenericParameterAntetype a => ReplaceTypeParametersIn(a),
            FunctionAntetype a => ReplaceTypeParametersIn(a),
            OptionalAntetype a => ReplaceTypeParametersIn(a),
            _ => throw ExhaustiveMatch.Failed(antetype)
        };

    private UserGenericNominalAntetype ReplaceTypeParametersIn(UserGenericNominalAntetype antetype)
    {
        var replacementTypeArguments = ReplaceTypeParametersIn(antetype.TypeArguments);
        if (ReferenceEquals(antetype.TypeArguments, replacementTypeArguments))
            return antetype;

        return new(antetype.DeclaredAntetype, replacementTypeArguments);
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

    private IAntetype ReplaceTypeParametersIn(GenericParameterAntetype antetype)
    {
        if (replacements.TryGetValue(antetype, out var replacementType))
            return replacementType;
        return antetype;
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
