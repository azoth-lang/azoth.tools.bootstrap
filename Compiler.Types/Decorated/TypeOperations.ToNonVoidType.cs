namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public static partial class TypeOperations
{
    /// <summary>
    /// Convert to a non-void type by replacing void with unknown.
    /// </summary>
    public static IMaybeNonVoidType ToNonVoidType(this IMaybeType type)
        => type as IMaybeNonVoidType ?? IType.Unknown;
}
