using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public static partial class TypeOperations
{
    public static string ToSourceCodeString(this IPromise<DataType?> promise)
        => promise.ToString(t => t?.ToSourceCodeString() ?? "⧼null⧽");

    public static string ToILString(this IFixedList<DataType> types)
        => string.Join(", ", types.Select(t => t.ToILString()));

    /// <summary>
    /// Create an optional type for the given type (i.e. `T?` given `T`).
    /// </summary>
    /// <remarks>Unknown and empty types (i.e. `void` and `never`) are not changed.</remarks>
    // TODO give a better name, this isn't really a conversion
    [return: NotNullIfNotNull(nameof(type))]
    public static DataType? ToOptional(this DataType? type)
        => type switch
        {
            null => null,
            UnknownType or NeverType or VoidType => type,
            Type t => new OptionalType(t),
            _ => throw new UnreachableException(),
        };

    internal static INumericType? AsNumericType(this NonEmptyType type)
        => type switch
        {
            ValueType { DeclaredType: NumericType t } => t,
            IntegerConstValueType t => t,
            _ => null,
        };
}
