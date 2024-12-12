using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

internal sealed class PlainTypeReplacements
{
    public static readonly PlainTypeReplacements None = new();

    private readonly Dictionary<GenericParameterPlainType, IPlainType> replacements;

    private PlainTypeReplacements()
    {
        replacements = new();
    }

    /// <summary>
    /// Build a dictionary of type replacements. Generic parameter types of both this type and the
    /// supertypes can be replaced with type arguments of this type.
    /// </summary>
    public PlainTypeReplacements(TypeConstructor typeConstructor, IFixedList<IPlainType> typeArguments)
    {
        replacements = typeConstructor.ParameterPlainTypes.EquiZip(typeArguments)
                                   .ToDictionary(t => t.Item1, t => t.Item2);
        if (typeConstructor.Parameters.IsEmpty)
            return;
        // Set up replacements for supertype generic parameters
        // TODO this might have been needed when inheritance was implemented by treating methods as
        //      if they were copied down the hierarchy, but I don't think it should be needed when
        //      they are properly handled.
        // TODO this also can't work because you can implement a trait multiple times with different type arguments
        //foreach (var supertype in typeConstructor.Supertypes)
        //    foreach (var (supertypeArgument, i) in supertype.Arguments.Enumerate())
        //    {
        //        var genericParameterPlainType = supertype.TypeConstructor?.ParameterPlainTypes[i];
        //        if (genericParameterPlainType is null)
        //            continue;
        //        if (supertypeArgument.PlainType is GenericParameterPlainType genericPlainTypeArg)
        //        {
        //            if (replacements.TryGetValue(genericPlainTypeArg, out var replacement))
        //                replacements.Add(genericParameterPlainType, replacement);
        //            else
        //                throw new InvalidOperationException(
        //                    $"Could not find replacement for `{supertypeArgument}` in type constructor `{typeConstructor}` with arguments `{typeArguments}`.");
        //        }
        //        else
        //            replacements.Add(genericParameterPlainType, ReplaceTypeParametersIn(supertypeArgument.PlainType));
        //    }
    }

    public IMaybePlainType ReplaceTypeParametersIn(IMaybePlainType plainType)
        => plainType switch
        {
            IPlainType a => ReplaceTypeParametersIn(a),
            UnknownPlainType a => a,
            _ => throw ExhaustiveMatch.Failed(plainType)
        };

    public IPlainType ReplaceTypeParametersIn(IPlainType plainType)
        => plainType switch
        {
            VoidPlainType a => a,
            NonVoidPlainType a => ReplaceTypeParametersIn(a),
            _ => throw ExhaustiveMatch.Failed(plainType)
        };

    public IPlainType ReplaceTypeParametersIn(NonVoidPlainType plainType)
        => plainType switch
        {
            NeverPlainType t => t,
            AssociatedPlainType t => t,
            ConstructedPlainType t => ReplaceTypeParametersIn(t),
            GenericParameterPlainType t => ReplaceTypeParametersIn(t),
            FunctionPlainType t => ReplaceTypeParametersIn(t),
            OptionalPlainType t => ReplaceTypeParametersIn(t),
            _ => throw ExhaustiveMatch.Failed(plainType)
        };

    public ConstructedOrAssociatedPlainType ReplaceTypeParametersIn(ConstructedOrAssociatedPlainType plainType)
        => plainType switch
        {
            ConstructedPlainType t => ReplaceTypeParametersIn(t),
            AssociatedPlainType t => t,
            _ => throw ExhaustiveMatch.Failed(plainType)
        };

    public ConstructedPlainType ReplaceTypeParametersIn(ConstructedPlainType plainType)
    {
        var replacementTypeArguments = ReplaceTypeParametersIn(plainType.Arguments);
        if (ReferenceEquals(plainType.Arguments, replacementTypeArguments))
            return plainType;

        return new(plainType.TypeConstructor, replacementTypeArguments);
    }

    private IFixedList<IPlainType> ReplaceTypeParametersIn(IFixedList<IPlainType> plainTypes)
    {
        var replacementPlainTypes = new List<IPlainType>();
        var typesReplaced = false;
        foreach (var plainType in plainTypes)
        {
            var replacementType = ReplaceTypeParametersIn(plainType);
            typesReplaced |= !ReferenceEquals(plainType, replacementType);
            replacementPlainTypes.Add(replacementType);
        }

        return typesReplaced ? replacementPlainTypes.ToFixedList() : plainTypes;
    }

    public IPlainType ReplaceTypeParametersIn(GenericParameterPlainType plainType)
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
    private IFixedList<NonVoidPlainType> ReplaceTypeParametersInParameters(
        IFixedList<NonVoidPlainType> plainTypes)
    {
        var replacementPlainTypes = new List<NonVoidPlainType>();
        var typesReplaced = false;
        foreach (var plainType in plainTypes)
        {
            var replacementType = ReplaceTypeParametersIn(plainType);
            typesReplaced |= !ReferenceEquals(plainType, replacementType);
            if (replacementType is NonVoidPlainType nonVoidPlainType)
                replacementPlainTypes.Add(nonVoidPlainType);
        }

        return typesReplaced ? replacementPlainTypes.ToFixedList() : plainTypes;
    }

    public IPlainType ReplaceTypeParametersIn(OptionalPlainType plainType)
    {
        var replacementType = ReplaceTypeParametersIn(plainType.Referent);
        if (ReferenceEquals(plainType.Referent, replacementType))
            return plainType;

        return replacementType.MakeOptional();
    }
}
