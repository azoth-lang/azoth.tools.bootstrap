namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

public partial class TypeOperations
{
    public static bool ReturnCanOverride(this IType returnType, IType baseReturnType)
        => baseReturnType.IsAssignableFrom(returnType);
}
