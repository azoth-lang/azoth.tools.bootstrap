using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

[Closed(
    typeof(SimpleTypeConstructor),
    typeof(LiteralTypeConstructor))]
public interface ISimpleOrLiteralTypeConstructor : ITypeConstructor
{
}
