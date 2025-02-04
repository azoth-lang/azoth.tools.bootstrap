using System;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Framework;

public static class BoolExtensions
{
    /// <summary>
    /// The logical relation implies (i.e. X implies Y means that if X is true then Y must be true).
    /// </summary>
    /// <remarks>This method is meant to be inlined. That means the <c>InlineMethod.Fody</c> package
    /// should be properly installed in the consuming code.</remarks>
    [Inline(export: true)]
    public static bool Implies(this bool self, bool other)
        => !self || other;

    /// <summary>
    /// The logical relation implies (i.e. X implies Y means that if X is true then Y must be true).
    /// This overload will short-circuit and not evaluate <paramref name="other"/> when not
    /// necessary.
    /// </summary>
    /// <remarks>This method is meant to be inlined. That means the <c>InlineMethod.Fody</c> package
    /// should be properly installed in the consuming code. However, a lambda parameter <i>may</i>
    /// still not be optimized away.</remarks>
    [Inline(export: true)]
    public static bool Implies(this bool self, Func<bool> other)
        => !self || other();
}
