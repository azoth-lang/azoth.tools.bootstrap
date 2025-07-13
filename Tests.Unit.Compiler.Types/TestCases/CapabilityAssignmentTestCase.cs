using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types.TestCases;

public record class CapabilityAssignmentTestCase(
    Capability From,
    Capability To,
    bool Assignable)
{
    public override string ToString() => Assignable ? $"{From} <: {To}" : $"{From} ≮: {To}";
}
