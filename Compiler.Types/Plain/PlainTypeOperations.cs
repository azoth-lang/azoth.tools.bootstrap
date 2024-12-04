using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;
public static partial class PlainTypeOperations
{
    public static IFixedList<INonVoidPlainType>? AsKnownFixedList(this IEnumerable<IMaybeNonVoidPlainType> parameters)
    {
        if (parameters is IFixedList<INonVoidPlainType> knownParameters) return knownParameters;

        return parameters.ToFixedList().As<INonVoidPlainType>();
    }

    /// <summary>
    /// Convert to a non-void type by replacing void with unknown.
    /// </summary>
    public static IMaybeNonVoidPlainType ToNonVoid(this IMaybePlainType type)
        => type as IMaybeNonVoidPlainType ?? IPlainType.Unknown;
}
