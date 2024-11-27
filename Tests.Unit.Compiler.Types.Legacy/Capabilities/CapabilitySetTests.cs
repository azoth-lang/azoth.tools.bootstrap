using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Capabilities;
using Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types.Legacy.TestCases;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types.Legacy.Capabilities;

public class CapabilitySetTests
{
    [Theory]
    [MemberData(nameof(Correct_UpperBound_Data))]
    public void Correct_UpperBound(CapabilitySetUpperBoundTestCase testCase)
    {
        var actual = testCase.CapabilitySet.UpperBound;

        Assert.Same(testCase.ExpectedUpperBound, actual);
    }

    public static TheoryData<CapabilitySetUpperBoundTestCase> Correct_UpperBound_Data()
    {
        var data = new TheoryData<CapabilitySetUpperBoundTestCase>
        {
            new(CapabilitySet.Readable, Capability.Read),
            new(CapabilitySet.Shareable, Capability.Identity),
            new(CapabilitySet.Aliasable, Capability.Identity),
            new(CapabilitySet.Sendable, Capability.Identity),
            new(CapabilitySet.Any, Capability.Identity),
        };
        return data;
    }
}
