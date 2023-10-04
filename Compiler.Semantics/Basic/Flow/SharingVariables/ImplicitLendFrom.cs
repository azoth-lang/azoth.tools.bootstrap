using System;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;

/// <summary>
/// The reference that is being lent from. That is, the side that is being borrowed.
/// </summary>
public class ImplicitLendFrom : ISharingVariable
{
    public ImplicitLend ImplicitLend { get; }
    public bool IsVariableOrParameter => false;
    /// <remarks>The from side is affected by any restrictions.</remarks>
    public bool RestrictsWrite => ImplicitLend.RestrictWrite;
    public bool SharingIsTracked => true;
    public bool KeepsSetAlive => true;
    public ImplicitLendTo To => ImplicitLend.To;

    public ImplicitLendFrom(ImplicitLend implicitLend)
    {
        ImplicitLend = implicitLend;
    }

    #region Equality
    public bool Equals(ISharingVariable? other)
        => other is ImplicitLendFrom lendFrom && ImplicitLend.Equals(lendFrom.ImplicitLend);

    public override bool Equals(object? obj) => obj is ImplicitLendFrom other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(typeof(ImplicitLendFrom), ImplicitLend);
    #endregion

    public override string ToString() => $"⧼lend-from{ImplicitLend.Number}⧽";
}
