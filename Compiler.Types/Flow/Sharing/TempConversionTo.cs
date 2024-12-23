using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Flow.Sharing;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class TempConversionTo : IConversion
{
    public static TempConversionTo Constant(ValueId id) => new(id, Capability.TemporarilyConstant);
    public static TempConversionTo Isolated(ValueId id) => new(id, Capability.TemporarilyIsolated);

    public ulong Number { get; }
    public Capability Capability { get; }
    public TempConversionFrom From { get; }

    /// <remarks>The conversion to side is NOT affected by any restrictions.</remarks>
    public CapabilityRestrictions RestrictionsImposed => CapabilityRestrictions.None;

    private TempConversionTo(ValueId id, Capability capability)
    {
        Number = id.Value;
        Capability = capability;
        From = new(this);
    }

    #region Equality
    public bool Equals(IConversion? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is TempConversionTo tempConversionTo
               && Number == tempConversionTo.Number
               && Capability == tempConversionTo.Capability;
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is TempConversionTo other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Number);
    #endregion

    public override string ToString() => $"⧼to{Number}⧽";
}
