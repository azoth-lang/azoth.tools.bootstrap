using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(
    typeof(SimpleType),
    typeof(OptionalType))]
public abstract class ValueType : NonEmptyType
{
    private protected ValueType() { }
}
