using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors.Contexts;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public static partial class PlainTypeOperations
{
    public static IFixedList<NonVoidPlainType>? AsKnownFixedList(this IEnumerable<IMaybeNonVoidPlainType> parameters)
    {
        if (parameters is IFixedList<NonVoidPlainType> knownParameters) return knownParameters;

        return parameters.ToFixedList().As<NonVoidPlainType>();
    }

    /// <summary>
    /// Convert to a non-void type by replacing void with unknown.
    /// </summary>
    public static IMaybeNonVoidPlainType ToNonVoid(this IMaybePlainType type)
        => type as IMaybeNonVoidPlainType ?? PlainType.Unknown;

    // TODO remove hack
    internal static bool IsStringType(this ConstructedPlainType self)
        => !self.TypeConstructor.HasParameters
           && self.TypeConstructor.Context is NamespaceContext sc
           && sc.Namespace == NamespaceName.Global;
}
