using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(
    typeof(OptionalPlainType),
    typeof(FunctionPlainType),
    typeof(ConstructedOrAssociatedPlainType),
    typeof(GenericParameterPlainType),
    typeof(NeverPlainType))]
public abstract class NonVoidPlainType : PlainType, IMaybeNonVoidPlainType
{
    /// <summary>
    /// The semantics of values of this type or <see langword="null"/> if it is a variable and the
    /// semantics are not known.
    /// </summary>
    public abstract TypeSemantics? Semantics { get; }
}
