using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

[Closed(
    typeof(IntegerTypeConstructor))]
public abstract class NumericTypeConstructor : SimpleTypeConstructor, INumericTypeConstructor
{
    private protected NumericTypeConstructor(SpecialTypeName name)
        : base(name) { }
}
