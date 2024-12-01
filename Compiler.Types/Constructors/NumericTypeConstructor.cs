using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

[Closed(
    typeof(IntegerTypeConstructor))]
public abstract class NumericTypeConstructor : SimpleTypeConstructor, INumericAntetype
{
    IExpressionAntetype INumericAntetype.Antetype => this;

    private protected NumericTypeConstructor(SpecialTypeName name)
        : base(name) { }
}
