using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// A type that at least in theory can be embodied by values.
/// </summary>
[Closed(
    typeof(ReferenceType),
    typeof(ValueType),
    typeof(GenericParameterType))]
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
    public ParameterType ReplaceTypeParametersIn(ParameterType type)
        => type with { Type = ReplaceTypeParametersIn(type.Type) };

    /// <summary>
    /// Replace any <see cref="GenericParameterType"/> from this type that appear in the given type
    /// with the type arguments from this type (assuming it has them).
    /// </summary>
    /// <remarks>Has no effect if this is not a generic type.</remarks>
    public ReturnType ReplaceTypeParametersIn(ReturnType returnType)
        => returnType with { Type = ReplaceTypeParametersIn(returnType.Type) };
}
