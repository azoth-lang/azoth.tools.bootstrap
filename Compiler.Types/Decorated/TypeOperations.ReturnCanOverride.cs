namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public partial class TypeOperations
{
    public static bool ReturnCanOverride(this Type returnType, Type baseReturnType)
        => returnType.IsSubtypeOf(baseReturnType, substitutable: true);
}
