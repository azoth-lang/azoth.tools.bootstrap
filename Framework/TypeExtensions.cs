using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Azoth.Tools.Bootstrap.Framework;

public static partial class TypeExtensions
{
    public static string GetFriendlyName(this Type type)
    {
        var name = type.Name;
        var fileScopeTypeNameMatch = FileScopeTypeNameRegex().Match(name);
        if (fileScopeTypeNameMatch.Success)
        {
            var file = fileScopeTypeNameMatch.Groups["file"];
            var typeName = fileScopeTypeNameMatch.Groups["name"];
            name = $"<{file}>.{typeName}";
        }

        if (type.IsGenericParameter || !type.IsGenericType)
            return name;

        var index = name.IndexOf('`');
        name = name[..index];
        var genericArguments = string.Join(',', type.GetGenericArguments().Select(GetFriendlyName));
        return $"{name}<{genericArguments}>";
    }

    [GeneratedRegex("^<(?<file>[a-zA-Z_]+)>[A-F0-9]+__(?<name>.*)$")]
    private static partial Regex FileScopeTypeNameRegex();

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
