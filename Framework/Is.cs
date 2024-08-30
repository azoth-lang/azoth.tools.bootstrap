using InlineMethod;

namespace Azoth.Tools.Bootstrap.Framework;

public static class Is
{
    [Inline(export: true)]
    public static T OfType<T>(this T value) => value;
}
