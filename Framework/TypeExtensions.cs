using System;
using System.Collections.Generic;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Framework;

public static class TypeExtensions
{
    public static string GetFriendlyName(this Type type)
    {
        if (type.IsGenericParameter || !type.IsGenericType)
            return type.Name;

        var name = type.Name;
        var index = name.IndexOf('`');
        name = name[..index];
        var genericArguments = string.Join(',', type.GetGenericArguments().Select(GetFriendlyName));
        return $"{name}<{genericArguments}>";
    }

    public static IEnumerable<Type> GetAllSubtypes(this Type type)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .Where(t => type.IsAssignableFrom(t) && t != type)
                        .ToArray();
    }

    public static bool HasCustomAttribute<TAttribute>(this Type type)
        => type.GetCustomAttributes(true).OfType<TAttribute>().Any();

    /// <summary>
    /// Whether this type <c>T</c> implements <c>IEquatable&lt;T&gt;</c>.
    /// </summary>
    public static bool IsEquatable(this Type type)
        => type.IsAssignableTo(typeof(IEquatable<>).MakeGenericType(type));

    public static bool IsEquatableToSupertype(this Type type)
        => type.IsEquatable() || type.GetInterfaces()
                                     .Any(i => i.IsGenericType
                                               && i.GetGenericTypeDefinition() == typeof(IEquatable<>)
                                               && i.GenericTypeArguments[0].IsAssignableFrom(type));

    /// <summary>
    /// Whether this type <c>T?</c> implements <c>IEquatable&lt;T&gt;</c>.
    /// </summary>
    public static bool IsNullableOfEquatable(this Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type);
        return underlyingType?.IsEquatable() ?? false;
    }
}
