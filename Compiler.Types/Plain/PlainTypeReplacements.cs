using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public sealed class PlainTypeReplacements
{
    public static readonly PlainTypeReplacements None = new();

    private readonly BarePlainType? selfReplacement;
    private readonly Dictionary<GenericParameterPlainType, PlainType> replacements = new();

    private PlainTypeReplacements() { }

    /// <summary>
    /// Build a dictionary of type replacements. Generic parameter types of both this type and the
    /// supertypes can be replaced with type arguments of this type.
    /// </summary>
    public PlainTypeReplacements(BarePlainType plainType)
    {
        selfReplacement = plainType;
        AddReplacements(plainType);
    }

    private void AddReplacements(BarePlainType plainType)
    {
        if (plainType.ContainingType is { } containingType)
            AddReplacements(containingType);
        var typeConstructor = plainType.TypeConstructor;
        foreach (var (parameter, arg) in typeConstructor.ParameterPlainTypes
                                                        .EquiZip(plainType.Arguments))
            if (!parameter.Equals(arg)) // No point in replacing with the same (happens for type constructed with their type parameters)
                replacements.Add(parameter, arg);

        // Set up replacements for supertype generic parameters
        // TODO this is needed because of the naive way that members are inherited without change. Instead,
        // they should be inherited with proper replacement and context. Then this can be removed.
        // Can't access plainType.Supertypes because that depends on the type replacements, use
        // `typeConstructor.Supertypes` instead.
        foreach (var supertype in typeConstructor.Supertypes)
            foreach (var (parameter, arg) in supertype.TypeConstructor.ParameterPlainTypes
                                                      .EquiZip(supertype.Arguments.Select(a => a.PlainType)))
            {
                var replacement = ApplyTo(arg);
                if (!replacements.TryAdd(parameter, replacement) && !replacements[parameter].Equals(replacement))
                    throw new NotImplementedException(
                        $"Conflicting type replacements. Replace `{parameter}` with "
                        + $"`{replacements[parameter]}` or `{replacement}`");
            }
    }

    public IMaybePlainType ApplyTo(IMaybePlainType plainType)
        => plainType switch
        {
            PlainType a => ApplyTo(a),
            UnknownPlainType a => a,
            _ => throw ExhaustiveMatch.Failed(plainType)
        };

    public PlainType ApplyTo(PlainType plainType)
        => plainType switch
        {
            VoidPlainType a => a,
            NonVoidPlainType a => ApplyTo(a),
            _ => throw ExhaustiveMatch.Failed(plainType)
        };

    public PlainType ApplyTo(NonVoidPlainType plainType)
        => plainType switch
        {
            NeverPlainType t => t,
            BarePlainType t => ApplyTo(t),
            GenericParameterPlainType t => ApplyTo(t),
            FunctionPlainType t => ApplyTo(t),
            OptionalPlainType t => ApplyTo(t),
            RefPlainType t => ApplyTo(t),
            _ => throw ExhaustiveMatch.Failed(plainType)
        };

    [return: NotNullIfNotNull(nameof(plainType))]
    public BarePlainType? ApplyTo(BarePlainType? plainType)
    {
        if (plainType is null) return null;
        if (selfReplacement is not null && plainType is { TypeConstructor: SelfTypeConstructor })
            return selfReplacement;

        var replacementContainingType = ApplyTo(plainType.ContainingType);
        var replacementTypeArguments = ApplyTo(plainType.Arguments);
        if (ReferenceEquals(plainType.ContainingType, replacementContainingType)
            && ReferenceEquals(plainType.Arguments, replacementTypeArguments))
            return plainType;

        return new(plainType.TypeConstructor, replacementContainingType, replacementTypeArguments);
    }

    private IFixedList<PlainType> ApplyTo(IFixedList<PlainType> plainTypes)
    {
        var replacementPlainTypes = new List<PlainType>();
        var typesReplaced = false;
        foreach (var plainType in plainTypes)
        {
            var replacementType = ApplyTo(plainType);
            typesReplaced |= !ReferenceEquals(plainType, replacementType);
            replacementPlainTypes.Add(replacementType);
        }

        return typesReplaced ? replacementPlainTypes.ToFixedList() : plainTypes;
    }

    public PlainType ApplyTo(GenericParameterPlainType plainType)
        => replacements.GetValueOrDefault(plainType, plainType);

    public FunctionPlainType ApplyTo(FunctionPlainType plainType)
    {
        var replacementParameterTypes = ApplyToParameters(plainType.Parameters);
        var replacementReturnType = ApplyTo(plainType.Return);
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
            var replacementType = ApplyTo(plainType);
            typesReplaced |= !ReferenceEquals(plainType, replacementType);
            if (replacementType is NonVoidPlainType nonVoidPlainType)
                replacementPlainTypes.Add(nonVoidPlainType);
        }

        return typesReplaced ? replacementPlainTypes.ToFixedList() : plainTypes;
    }

    public PlainType ApplyTo(OptionalPlainType plainType)
    {
        var replacementType = ApplyTo(plainType.Referent);
        if (ReferenceEquals(plainType.Referent, replacementType))
            return plainType;

        return OptionalPlainType.Create(replacementType);
    }

    public PlainType ApplyTo(RefPlainType plainType)
    {
        var replacementType = ApplyTo(plainType.Referent);
        if (ReferenceEquals(plainType.Referent, replacementType))
            return plainType;

        return RefPlainType.Create(plainType.IsInternal, plainType.IsMutableBinding, replacementType);
    }
}
