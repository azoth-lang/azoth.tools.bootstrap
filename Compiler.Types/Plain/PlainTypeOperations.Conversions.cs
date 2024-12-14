using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public static partial class PlainTypeOperations
{
    /// <summary>
    /// Create an optional type for the given type (i.e. `T?` given `T`).
    /// </summary>
    /// <remarks>Unknown, `void` and `never` types are not changed.</remarks>
    public static IMaybePlainType MakeOptional(this IMaybePlainType plainType)
        => plainType switch
        {
            UnknownPlainType or NeverPlainType or VoidPlainType => plainType,
            NonVoidPlainType t => new OptionalPlainType(t),
            _ => throw new UnreachableException(),
        };

    /// <summary>
    /// Create an optional type for the given type (i.e. `T?` given `T`).
    /// </summary>
    /// <remarks>`void` and `never` types are not changed.</remarks>
    public static PlainType MakeOptional(this PlainType plainType)
        => plainType switch
        {
            NeverPlainType or VoidPlainType => plainType,
            NonVoidPlainType t => new OptionalPlainType(t),
            _ => throw new UnreachableException(),
        };
}
