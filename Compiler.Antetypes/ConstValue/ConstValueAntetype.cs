using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes.ConstValue;

public abstract class ConstValueAntetype : IExpressionAntetype
{
    public SpecialTypeName Name { get; }

    private protected ConstValueAntetype(SpecialTypeName name)
    {
        Name = name;
    }
}
