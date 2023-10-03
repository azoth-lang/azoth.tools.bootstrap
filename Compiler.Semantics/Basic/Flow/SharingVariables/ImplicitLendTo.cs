using System;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;

/// <summary>
/// The reference that is being lent to. That is, the side that is borrowing.
/// </summary>
public class ImplicitLendTo : ISharingVariable
{
    public ImplicitLend ImplicitLend { get; }
    public bool IsVariableOrParameter => false;
    /// <remarks>The to side is NOT affected by any restrictions.</remarks>
    public bool RestrictsWrite => false;
    public bool IsTracked => true;
    public bool KeepsSetAlive => false;
    public ImplicitLendFrom From => ImplicitLend.From;

    public ImplicitLendTo(ImplicitLend implicitLend)
    {
        ImplicitLend = implicitLend;
    }

    #region Equality
    public bool Equals(ISharingVariable? other) =>
        other is ImplicitLendTo lendTo && ImplicitLend.Equals(lendTo.ImplicitLend);

    public override bool Equals(object? obj) => obj is ImplicitLendTo other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(typeof(ImplicitLendTo), ImplicitLend);
    #endregion

    public override string ToString() => $"⧼lend-to{ImplicitLend.Number}⧽";
}
