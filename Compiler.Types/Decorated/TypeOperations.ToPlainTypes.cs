using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public static partial class TypeOperations
{
    public static IFixedList<NonVoidPlainType> ToPlainTypes(this IEnumerable<ParameterType> parameters)
        => parameters.Select(p => p.PlainType).ToFixedList();
}
