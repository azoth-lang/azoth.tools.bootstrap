using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

public abstract class NumericAntetype : SimpleAntetype, INumericAntetype
{
    IExpressionAntetype INumericAntetype.Antetype => this;

    private protected NumericAntetype(SpecialTypeName name)
        : base(name) { }
}
