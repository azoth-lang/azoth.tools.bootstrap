using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

// e.g. `readable Self`
// Applies to SelfPlainType
// Could be CapabilitySelfType and operate on an ICapabilityConstraint but that
// would introduce two ways of having types with capabilities on them.
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public sealed class CapabilitySetSelfType : INonVoidType
{
    public CapabilitySet Capability { get; }
    public SelfPlainType SelfType { get; }
    INonVoidPlainType INonVoidType.PlainType => SelfType;

    public CapabilitySetSelfType(CapabilitySet capability, SelfPlainType selfType)
    {
        Capability = capability;
        SelfType = selfType;
    }

    public override string ToString() => throw new NotSupportedException();

    public string ToSourceCodeString() => $"{Capability.ToSourceCodeString()} {SelfType}";

    public string ToILString() => $"{Capability.ToILString()} {SelfType}";
}
