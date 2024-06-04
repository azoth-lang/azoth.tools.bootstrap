using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Compiler.Antetypes.ConstValue;

public sealed class BoolConstValueAntetype : ConstValueAntetype
{
    internal static readonly BoolConstValueAntetype True = new(true);
    internal static readonly BoolConstValueAntetype False = new(false);

    public bool Value { get; }

    private BoolConstValueAntetype(bool value)
        : base(value ? SpecialTypeName.True : SpecialTypeName.False)
    {
        Value = value;
    }
}
