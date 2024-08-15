using System;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;

/// <summary>
/// The reference that is being temporarily converted to. That is, the side that is borrowing.
/// </summary>
public class TempConversionTo : ISharingVariable
{
    public TempConversion TempConversion { get; }

    /// <remarks>The to side is NOT affected by any restrictions.</remarks>
    public CapabilityRestrictions RestrictionsImposed => CapabilityRestrictions.None;

    public bool KeepsSetAlive => false;
    public TempConversionFrom From => TempConversion.From;

    public TempConversionTo(TempConversion tempConversion)
    {
        TempConversion = tempConversion;
    }

    #region Equality
    public bool Equals(ISharingVariable? other) =>
        other is TempConversionTo lendTo && TempConversion.Equals(lendTo.TempConversion);

    public override bool Equals(object? obj) => obj is TempConversionTo other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(typeof(TempConversionTo), TempConversion);
    #endregion

    public override string ToString() => $"⧼temp-to{TempConversion.Number}⧽";
}
