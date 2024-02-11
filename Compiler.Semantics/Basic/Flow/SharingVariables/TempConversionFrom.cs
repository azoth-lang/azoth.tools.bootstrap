using System;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;

/// <summary>
/// The reference that is being temporarily converted from. That is, the side that is being borrowed.
/// </summary>
public class TempConversionFrom : ISharingVariable
{
    public TempConversion TempConversion { get; }
    public bool IsVariableOrParameter => false;
    /// <remarks>The from side is affected by any restrictions.</remarks>
    public CapabilityRestrictions RestrictionsImposed => TempConversion.RestrictionsImposed;
    public bool SharingIsTracked => true;
    public bool KeepsSetAlive => true;
    public TempConversionTo To => TempConversion.To;

    public TempConversionFrom(TempConversion tempConversion)
    {
        TempConversion = tempConversion;
    }

    #region Equality
    public bool Equals(ISharingVariable? other)
        => other is TempConversionFrom lendFrom && TempConversion.Equals(lendFrom.TempConversion);

    public override bool Equals(object? obj) => obj is TempConversionFrom other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(typeof(TempConversionFrom), TempConversion);
    #endregion

    public override string ToString() => $"⧼temp-from{TempConversion.Number}⧽";
}
