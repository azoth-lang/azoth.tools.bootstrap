using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

// TODO this should probably be removed. It seems to be used only for improperly handling operators yielding these types
[Closed(
    typeof(SimpleTypeConstructor),
    typeof(LiteralTypeConstructor))]
public interface SimpleOrLiteralTypeConstructor : TypeConstructor
{
}
