using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

public static partial class TypeOperations
{
    public static IFixedList<IMaybeNonVoidPlainType> ToPlainTypes(this IEnumerable<IMaybeParameterType> parameters)
        => parameters.Select(p => p.ToPlainType()).ToFixedList();

    public static IFixedList<IMaybeNonVoidPlainType> ToPlainTypes(this IEnumerable<Decorated.ParameterType> parameters)
        => parameters.Select(p => p.PlainType).ToFixedList();
}
