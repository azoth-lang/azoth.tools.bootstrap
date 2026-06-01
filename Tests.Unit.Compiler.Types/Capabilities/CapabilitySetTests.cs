using Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types.TestCases;
using Xunit;
using static Azoth.Tools.Bootstrap.Compiler.Types.Capabilities.Capability;
using static Azoth.Tools.Bootstrap.Compiler.Types.Capabilities.CapabilitySet;

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
            new(Readable, Read),
            new(Shareable, Identity),
            new(Aliasable, Identity),
            new(Sendable, Identity),
            new(Any, Identity),
        };
        return data;
    }
}
