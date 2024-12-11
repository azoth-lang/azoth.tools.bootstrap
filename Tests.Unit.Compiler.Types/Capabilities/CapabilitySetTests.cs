using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types.TestCases;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types.Capabilities;

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
