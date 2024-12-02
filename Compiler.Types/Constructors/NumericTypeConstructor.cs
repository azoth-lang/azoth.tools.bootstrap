using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

[Closed(
    typeof(IntegerTypeConstructor))]
public abstract class NumericTypeConstructor : SimpleTypeConstructor, INumericPlainType
{
    IPlainType INumericPlainType.PlainType => this;

    private protected NumericTypeConstructor(SpecialTypeName name)
        : base(name) { }
}
