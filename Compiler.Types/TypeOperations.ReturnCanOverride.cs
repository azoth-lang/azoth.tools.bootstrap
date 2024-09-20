namespace Azoth.Tools.Bootstrap.Compiler.Types;

public partial class TypeOperations
{
    public static bool ReturnCanOverride(this IType returnType, IType baseReturnType)
        => ((Type)baseReturnType).IsAssignableFrom((Type)returnType);
}
