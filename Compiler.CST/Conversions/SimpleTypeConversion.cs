using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions;

/// <summary>
/// Conversion between numeric types. For example `int32` to `int64`.
/// </summary>
public sealed class SimpleTypeConversion : ChainedConversion
{
    public SimpleType To { [DebuggerStepThrough] get; }

    public SimpleTypeConversion(SimpleType to, Conversion priorConversion)
        : base(priorConversion)
    {
        To = to;
    }

    public override DataType Apply(DataType type)
    {
        type = PriorConversion.Apply(type);
        // TODO check that the incoming type can work with numeric conversion
        return To.Type;
    }
}
