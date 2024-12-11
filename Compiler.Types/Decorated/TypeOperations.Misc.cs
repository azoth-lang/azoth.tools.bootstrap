using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public static partial class TypeOperations
{
    public static string ToILString(this IFixedList<IMaybeType> types)
        => string.Join(", ", types.Select(t => t.ToILString()));
}
