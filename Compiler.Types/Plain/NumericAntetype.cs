using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(
    typeof(IntegerAntetype))]
public abstract class NumericAntetype : SimpleAntetype, INumericAntetype
{
    IExpressionAntetype INumericAntetype.Antetype => this;

    private protected NumericAntetype(SpecialTypeName name)
        : base(name) { }
}
