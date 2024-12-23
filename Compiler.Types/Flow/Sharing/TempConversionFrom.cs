using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Flow.Sharing;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class TempConversionFrom : IConversion
{
    private readonly TempConversionTo to;
    public ulong Number => to.Number;
    public CapabilityRestrictions RestrictionsImposed { get; }

    public TempConversionFrom(TempConversionTo to)
    {
        this.to = to;
        if (to.Capability == Capability.TemporarilyConstant)
            RestrictionsImposed = CapabilityRestrictions.Write;
        else if (to.Capability == Capability.TemporarilyIsolated)
            RestrictionsImposed = CapabilityRestrictions.ReadWrite;
        else
            throw new ArgumentException("Must be a temporarily capability.", nameof(to));
    }

    #region Equality
    public bool Equals(IConversion? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is TempConversionFrom tempConversionFrom
               && to.Equals(tempConversionFrom.to);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is TempConversionFrom other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(to);
    #endregion

    public override string ToString() => $"⧼from{Number}⧽";
}
