using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

// e.g. `self |> mut Foo` Applies to all non-void types
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public sealed class SelfViewpointType : INonVoidType
{
    public CapabilitySet Capability { get; }

    public INonVoidType Referent { get; }

    public INonVoidPlainType PlainType => Referent.PlainType;

    public SelfViewpointType(CapabilitySet capability, INonVoidType referent)
    {
        Capability = capability;
        Referent = referent;
    }

    #region Equality
    public bool Equals(IMaybeType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is SelfViewpointType otherType
               && Capability.Equals(otherType.Capability)
               && Referent.Equals(otherType.Referent);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is SelfViewpointType other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Capability, Referent);
    #endregion

    public override string ToString() => throw new NotSupportedException();

    public string ToSourceCodeString() => $"{Capability.ToSourceCodeString()} self |> {Referent}";

    public string ToILString() => $"{Capability.ToILString()} self |> {Referent}";
}
