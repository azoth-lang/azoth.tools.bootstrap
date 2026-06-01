using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types.TestCases;

public record class CapabilitySetWithIdentityIntersectTestCase(
    CapabilitySetWithIdentity Left,
    CapabilitySetWithIdentity Right,
    CapabilitySetWithIdentity Expected)
{
    public override string ToString() => $"{Left} ∩ {Right} = {Expected}";
}
