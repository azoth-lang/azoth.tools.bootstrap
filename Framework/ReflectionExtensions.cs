using System.Linq;
using System.Reflection;

namespace Azoth.Tools.Bootstrap.Framework;

public static class ReflectionExtensions
{
    public static PropertyInfo? GetProperty(this MethodBase methodBase)
        => methodBase is not MethodInfo method ? null : GetProperty(method);

    private static PropertyInfo? GetProperty(MethodInfo method)
    {
        var declaringType = method.DeclaringType;
        if (declaringType is null) return null;
        bool takesArg = method.GetParameters().Length == 1;
        bool hasReturn = method.ReturnType != typeof(void);
        if (takesArg == hasReturn) return null;

        var properties = declaringType.GetProperties();
        if (takesArg) return properties.FirstOrDefault(prop => prop.GetSetMethod() == method);

        return properties.FirstOrDefault(prop => prop.GetGetMethod() == method);
    }
}
