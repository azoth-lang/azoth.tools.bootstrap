using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
internal sealed class UnsetAttribute
{
    #region Singleton
    public static readonly UnsetAttribute Instance = new();

    private UnsetAttribute() { }
    #endregion

    public override string ToString() => "<<unset>>";
}
