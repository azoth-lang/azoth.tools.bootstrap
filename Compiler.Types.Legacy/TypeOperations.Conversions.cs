using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

public static partial class TypeOperations
{
    public static string ToILString(this IFixedList<IMaybeType> types)
        => string.Join(", ", types.Select(t => t.ToILString()));

    /// <summary>
    /// Convert to a non-void type by replacing void with unknown.
    /// </summary>
    public static IMaybeNonVoidType ToNonVoidType(this IMaybeType type)
        => type as IMaybeNonVoidType ?? IType.Unknown;
}
