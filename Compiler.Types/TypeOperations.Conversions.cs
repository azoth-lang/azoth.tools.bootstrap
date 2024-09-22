using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public static partial class TypeOperations
{
    public static string ToILString(this IFixedList<IMaybeExpressionType> types)
        => string.Join(", ", types.Select(t => t.ToILString()));

    internal static INumericType? AsNumericType(this NonEmptyType type)
        => type switch
        {
            CapabilityType { DeclaredType: NumericType t } => t,
            IntegerConstValueType t => t,
            _ => null,
        };
}
