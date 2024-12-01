using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

[Closed(
    typeof(IntegerAntetype))]
public abstract class NumericAntetype : SimpleAntetype, INumericAntetype
{
    IExpressionAntetype INumericAntetype.Antetype => this;

    private protected NumericAntetype(SpecialTypeName name)
        : base(name) { }
}
