using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types.TestCases;

public record class CapabilitySubtypeTestCase(
    Capability Left,
    Capability Right,
    bool IsSubtype)
{
    public override string ToString() => IsSubtype ? $"{Left} <: {Right}" : $"{Left} ≮: {Right}";
}
