using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;

/// <summary>
/// A conversion to a temporary capability (e.g. a temporary freeze or temporary move).
/// </summary>
public class TempConversion
{
    public static TempConversion CreateTempFreeze(TempConversion? currentConversion)
        => new TempConversion(currentConversion, CapabilityRestrictions.Write, ReferenceCapability.TemporarilyConstant);

    public static TempConversion CreateTempMove(TempConversion? currentConversion)
        => new TempConversion(currentConversion, CapabilityRestrictions.ReadWrite, ReferenceCapability.TemporarilyIsolated);

    public ulong Number { get; }
    public CapabilityRestrictions RestrictionsImposed { get; }
    public ReferenceCapability ConvertTo { get; }
    public TempConversionFrom From { get; set; }
    public TempConversionTo To { get; set; }

    private TempConversion(TempConversion? currentConversion, CapabilityRestrictions restrictionsImposed, ReferenceCapability convertTo)
    {
        if (!convertTo.AllowsSequesteredAliases)
            throw new ArgumentException("Must be a temporarily capability.", nameof(convertTo));
        Number = currentConversion?.Number + 1 ?? 0;
        RestrictionsImposed = restrictionsImposed;
        ConvertTo = convertTo;
        From = new TempConversionFrom(this);
        To = new TempConversionTo(this);
    }
}
