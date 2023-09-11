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

    public virtual DataType ReplaceTypeParametersIn(DataType type) => type;
}
