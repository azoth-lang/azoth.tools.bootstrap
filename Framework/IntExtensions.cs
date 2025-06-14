using InlineMethod;

namespace Azoth.Tools.Bootstrap.Framework;

public static class IntExtensions
{
    /// <summary>
    /// A short-circuiting combination of comparisons
    /// </summary>
    /// <remarks><para>This method is meant to be inlined. That means the <c>InlineMethod.Fody</c>
    /// package should be properly installed in the consuming code. When inlined, the method will
    /// have short-circuiting behavior like `&&` or `||`.</para>
    ///
    /// <para>CAUTION: If this method is not inlined, it will not short-circuit.</para></remarks>
    [Inline(export: true)]
    public static int ThenCompareBy(this int comparison, [InlineParameter] int nextComparison)
        => comparison != 0 ? comparison : nextComparison;
}
