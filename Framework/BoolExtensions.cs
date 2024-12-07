namespace Azoth.Tools.Bootstrap.Framework;

public static class BoolExtensions
{
    public static bool Implies(this bool self, bool other)
        => !self || other;
}
