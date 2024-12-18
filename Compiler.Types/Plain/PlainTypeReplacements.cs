using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public sealed class PlainTypeReplacements
{
    public static readonly PlainTypeReplacements None = new();

    private readonly ConstructedPlainType? selfReplacement;
    private readonly Dictionary<GenericParameterPlainType, PlainType> replacements = new();

    private PlainTypeReplacements() { }

    /// <summary>
    /// Build a dictionary of type replacements. Generic parameter types of both this type and the
    /// supertypes can be replaced with type arguments of this type.
    /// </summary>
    public PlainTypeReplacements(ConstructedPlainType plainType)
    {
        selfReplacement = plainType;
        AddReplacements(plainType);
    }

    private void AddReplacements(ConstructedPlainType plainType)
    {
        if (plainType.ContainingType is { } containingType)
            AddReplacements(containingType);
        foreach (var (parameter, arg) in plainType.TypeConstructor.ParameterPlainTypes.EquiZip(plainType.Arguments))
            replacements.Add(parameter, arg);
    }

    public IMaybePlainType Apply(IMaybePlainType plainType)
        => plainType switch
        {
            PlainType a => Apply(a),
            UnknownPlainType a => a,
            _ => throw ExhaustiveMatch.Failed(plainType)
        };

    public PlainType Apply(PlainType plainType)
        => plainType switch
        {
            VoidPlainType a => a,
            NonVoidPlainType a => Apply(a),
            _ => throw ExhaustiveMatch.Failed(plainType)
        };

    public PlainType Apply(NonVoidPlainType plainType)
        => plainType switch
        {
            NeverPlainType t => t,
            ConstructedPlainType t => Apply(t),
            GenericParameterPlainType t => Apply(t),
            FunctionPlainType t => Apply(t),
            OptionalPlainType t => Apply(t),
            _ => throw ExhaustiveMatch.Failed(plainType)
        };

    [return: NotNullIfNotNull(nameof(plainType))]
    public ConstructedPlainType? Apply(ConstructedPlainType? plainType)
    {
        if (plainType is null) return null;
        if (selfReplacement is not null && plainType is { TypeConstructor: SelfTypeConstructor })
            return selfReplacement;

        var replacementContainingType = Apply(plainType.ContainingType);
        var replacementTypeArguments = Apply(plainType.Arguments);
        if (ReferenceEquals(plainType.ContainingType, replacementContainingType)
            && ReferenceEquals(plainType.Arguments, replacementTypeArguments))
            return plainType;

        return new(plainType.TypeConstructor, replacementContainingType, replacementTypeArguments);
    }

    private IFixedList<PlainType> Apply(IFixedList<PlainType> plainTypes)
    {
        var replacementPlainTypes = new List<PlainType>();
        var typesReplaced = false;
        foreach (var plainType in plainTypes)
        {
            var replacementType = Apply(plainType);
            typesReplaced |= !ReferenceEquals(plainType, replacementType);
            replacementPlainTypes.Add(replacementType);
        }

        return typesReplaced ? replacementPlainTypes.ToFixedList() : plainTypes;
    }

    public PlainType Apply(GenericParameterPlainType plainType)
        => replacements.GetValueOrDefault(plainType, plainType);

    public FunctionPlainType Apply(FunctionPlainType plainType)
    {
        var replacementParameterTypes = ApplyToParameters(plainType.Parameters);
        var replacementReturnType = Apply(plainType.Return);
        if (ReferenceEquals(plainType.Parameters, replacementParameterTypes)
            && ReferenceEquals(plainType.Return, replacementReturnType))
            return plainType;

        return new(replacementParameterTypes, replacementReturnType);
    }

    /// <summary>
    /// Replace type parameters specifically in parameters to a function where `void` can cause
    /// a parameter to drop out.
    /// </summary>
    private IFixedList<NonVoidPlainType> ApplyToParameters(
        IFixedList<NonVoidPlainType> plainTypes)
    {
        var replacementPlainTypes = new List<NonVoidPlainType>();
        var typesReplaced = false;
        foreach (var plainType in plainTypes)
        {
            var replacementType = Apply(plainType);
            typesReplaced |= !ReferenceEquals(plainType, replacementType);
            if (replacementType is NonVoidPlainType nonVoidPlainType)
                replacementPlainTypes.Add(nonVoidPlainType);
        }

        return typesReplaced ? replacementPlainTypes.ToFixedList() : plainTypes;
    }

    public PlainType Apply(OptionalPlainType plainType)
    {
        var replacementType = Apply(plainType.Referent);
        if (ReferenceEquals(plainType.Referent, replacementType))
            return plainType;

        return replacementType.MakeOptional();
    }
}
