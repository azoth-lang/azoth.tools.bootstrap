using InlineMethod;

namespace Azoth.Tools.Bootstrap.Framework;

public static class BoolExtensions
{
    [Inline(export: true)]
    public static bool Implies(this bool self, bool other)
        => !self || other;
}
