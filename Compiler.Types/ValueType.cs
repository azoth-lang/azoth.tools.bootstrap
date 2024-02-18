using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(
    typeof(SimpleType))]
public abstract class ValueType : NonEmptyType
{
    private protected ValueType() { }
}
