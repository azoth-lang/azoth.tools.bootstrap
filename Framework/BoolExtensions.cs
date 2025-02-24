using InlineMethod;

namespace Azoth.Tools.Bootstrap.Framework;

public static class BoolExtensions
{
    /// <summary>
    /// The logical relation implies (i.e. X implies Y means that if X is true then Y must be true).
    /// </summary>
    /// <remarks><para>This method is meant to be inlined. That means the <c>InlineMethod.Fody</c>
    /// package should be properly installed in the consuming code. When inlined, the method will
    /// have short-circuiting behavior like `&&` or `||`.</para>
    ///
    /// <para>CAUTION: If this method is not inlined, it will not short-circuit.</para></remarks>
    [Inline(export: true)]
    public static bool Implies(this bool self, [InlineParameter] bool other)
    {
        // Must use `if` instead of `||` so C# compiler will not optimize away the short-circuiting
        // and make the inline work incorrectly.
        if (!self) return true;
        return other;
    }
}
