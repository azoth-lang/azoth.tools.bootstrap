namespace Azoth.Tools.Bootstrap.Compiler.Types;

public partial class TypeOperations
{
    public static bool ReturnCanOverride(this DataType returnType, DataType baseReturnType)
        => baseReturnType.IsAssignableFrom(returnType);
}
