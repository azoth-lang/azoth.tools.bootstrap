using InlineMethod;

namespace Azoth.Tools.Bootstrap.Framework;

public static class Is
{
    /// <summary>
    /// A hopefully zero overhead way of enforcing that a value has a certain type.
    /// </summary>
    /// <remarks>This is used by the attribute grammar code gen to enforce that user provided
    /// expressions are of the correct type when the context may not enforce the proper type.</remarks>
    [Inline(export: true)]
    public static T OfType<T>(this T value) => value;
}
