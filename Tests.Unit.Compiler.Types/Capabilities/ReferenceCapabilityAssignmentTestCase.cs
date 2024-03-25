using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types.Capabilities;

public record class ReferenceCapabilityAssignmentTestCase(
    Capability From,
    Capability To,
    bool Assignable)
{
    public override string ToString() => Assignable ? $"{From} <: {To}" : $"{From} ≮: {To}";
}
