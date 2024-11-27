using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public static partial class AntetypeOperations
{
    /// <summary>
    /// Create an optional type for the given type (i.e. `T?` given `T`).
    /// </summary>
    /// <remarks>Unknown, `void` and `never` types are not changed.</remarks>
    public static IMaybeAntetype MakeOptional(this IMaybeAntetype antetype)
        => antetype switch
        {
            UnknownAntetype or NeverAntetype or VoidAntetype => antetype,
            INonVoidAntetype t => new OptionalAntetype(t),
            _ => throw new UnreachableException(),
        };

    /// <summary>
    /// Create an optional type for the given type (i.e. `T?` given `T`).
    /// </summary>
    /// <remarks>`void` and `never` types are not changed.</remarks>
    public static IAntetype MakeOptional(this IAntetype antetype)
        => antetype switch
        {
            NeverAntetype or VoidAntetype => antetype,
            INonVoidAntetype t => new OptionalAntetype(t),
            _ => throw new UnreachableException(),
        };
}
