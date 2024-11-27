using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Capabilities;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types.Legacy.TestCases;

public record class CapabilitySetUpperBoundTestCase(CapabilitySet CapabilitySet, Capability ExpectedUpperBound)
{
    public override string ToString()
        => $"{CapabilitySet.ToILString()} <: {ExpectedUpperBound.ToILString()}";
}
