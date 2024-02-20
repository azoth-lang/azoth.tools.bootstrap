using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// A type that at least in theory can be embodied by values.
/// </summary>
[Closed(
    typeof(CapabilityType),
    typeof(GenericParameterType),
    typeof(ViewpointType),
    typeof(FunctionType),
    typeof(OptionalType),
    typeof(ConstValueType))]
public abstract class NonEmptyType : DataType
{
    private protected NonEmptyType() { }

    /// <summary>
    /// Replace any <see cref="GenericParameterType"/> from this type that appear in the given type
    /// with the type arguments from this type (assuming it has them).
    /// </summary>
    /// <remarks>Has no effect if this is not a generic type.</remarks>
    public virtual DataType ReplaceTypeParametersIn(DataType type) => type;

    /// <summary>
    /// Replace any <see cref="GenericParameterType"/> from this type that appear in the given type
    /// with the type arguments from this type (assuming it has them).
    /// </summary>
    /// <remarks>Has no effect if this is not a generic type.</remarks>
    public virtual Pseudotype ReplaceTypeParametersIn(Pseudotype pseudotype) => pseudotype;

    /// <summary>
    /// Replace any <see cref="GenericParameterType"/> from this type that appear in the given type
    /// with the type arguments from this type (assuming it has them).
    /// </summary>
    /// <remarks>Has no effect if this is not a generic type.</remarks>
    public Parameter ReplaceTypeParametersIn(Parameter type)
        => type with { Type = ReplaceTypeParametersIn(type.Type) };

    /// <summary>
    /// Replace any <see cref="GenericParameterType"/> from this type that appear in the given type
    /// with the type arguments from this type (assuming it has them).
    /// </summary>
    /// <remarks>Has no effect if this is not a generic type.</remarks>
    [return: NotNullIfNotNull(nameof(type))]
    public Parameter? ReplaceTypeParametersIn(Parameter? type)
    {
        if (type is Parameter parameterType)
            return ReplaceTypeParametersIn(parameterType);
        return null;
    }

    /// <summary>
    /// Replace any <see cref="GenericParameterType"/> from this type that appear in the given type
    /// with the type arguments from this type (assuming it has them).
    /// </summary>
    /// <remarks>Has no effect if this is not a generic type.</remarks>
    public SelfParameter ReplaceTypeParametersIn(SelfParameter type)
        => type with { Type = ReplaceTypeParametersIn(type.Type) };

    /// <summary>
    /// Replace any <see cref="GenericParameterType"/> from this type that appear in the given type
    /// with the type arguments from this type (assuming it has them).
    /// </summary>
    /// <remarks>Has no effect if this is not a generic type.</remarks>
    [return: NotNullIfNotNull(nameof(type))]
    public SelfParameter? ReplaceTypeParametersIn(SelfParameter? type)
    {
        if (type is SelfParameter parameterType)
            return ReplaceTypeParametersIn(parameterType);
        return null;
    }

    /// <summary>
    /// Replace any <see cref="GenericParameterType"/> from this type that appear in the given type
    /// with the type arguments from this type (assuming it has them).
    /// </summary>
    /// <remarks>Has no effect if this is not a generic type.</remarks>
    public Return ReplaceTypeParametersIn(Return @return)
        => @return with { Type = ReplaceTypeParametersIn(@return.Type) };
}
