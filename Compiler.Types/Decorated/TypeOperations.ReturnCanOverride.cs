namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public partial class TypeOperations
{
    public static bool ReturnCanOverride(this IType returnType, IType baseReturnType)
        => returnType.IsSubtypeOf(baseReturnType);
}
