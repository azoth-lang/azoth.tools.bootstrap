using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(
    typeof(OptionalPlainType),
    typeof(FunctionPlainType),
    typeof(BarePlainType),
    typeof(GenericParameterPlainType),
    typeof(NeverPlainType))]
public abstract class NonVoidPlainType : PlainType, IMaybeNonVoidPlainType
{
    public sealed override NonVoidPlainType ToNonLiteral() => TryToNonLiteral() ?? this;
    IMaybeNonVoidPlainType IMaybeNonVoidPlainType.ToNonLiteral() => ToNonLiteral();
}
