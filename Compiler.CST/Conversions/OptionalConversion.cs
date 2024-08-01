using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions;

/// <summary>
/// A conversion from `T` to `T?`
/// </summary>
// TODO this is odd since isn't `T` a subtype of `T?`?
public sealed class OptionalConversion : ChainedConversion
{
    public OptionalConversion(Conversion priorConversion) : base(priorConversion)
    {
    }

    public override DataType Apply(DataType type)
    {
        type = PriorConversion.Apply(type);
        return OptionalType.Create(type);
    }
}
