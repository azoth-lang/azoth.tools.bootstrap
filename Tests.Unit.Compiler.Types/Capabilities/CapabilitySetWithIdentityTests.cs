using Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types.TestCases;
using Xunit;
using static Azoth.Tools.Bootstrap.Compiler.Types.Capabilities.CapabilitySetWithIdentity;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types.Capabilities;

public class CapabilitySetWithIdentityTests
{
    [Theory]
    [MemberData(nameof(Intersect_Data))]
    public void Intersect(CapabilitySetWithIdentityIntersectTestCase testCase)
    {
        Assert.Same(testCase.Expected, testCase.Left.Intersect(testCase.Right));
        Assert.Same(testCase.Expected, testCase.Right.Intersect(testCase.Left));
    }

    public static TheoryData<CapabilitySetWithIdentityIntersectTestCase> Intersect_Data()
    {
        var data = new TheoryData<CapabilitySetWithIdentityIntersectTestCase>
        {
            new(Shareable, Shareable, Shareable),
            new(Shareable, Aliasable, Shareable),
            new(Shareable, Sendable, Shareable),
            new(Shareable, ReadOnly, Shareable),
            new(Shareable, Any, Shareable),

            new(Aliasable, Aliasable, Aliasable),
            new(Aliasable, Sendable, Shareable),
            new(Aliasable, ReadOnly, ReadOnly),
            new(Aliasable, Any, Aliasable),

            new(Sendable, Sendable, Sendable),
            new(Sendable, ReadOnly, Shareable),
            new(Sendable, Any, Sendable),

            new(ReadOnly, ReadOnly, ReadOnly),
            new(ReadOnly, Any, ReadOnly),

            new(Any, Any, Any),
        };
        return data;
    }
}
