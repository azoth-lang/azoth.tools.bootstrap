using Xunit;
using static Azoth.Tools.Bootstrap.Compiler.Types.ReferenceCapability;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types;

[Trait("Category", "Types")]
public class ReferenceCapabilityTests
{
    [Theory]
    [MemberData(nameof(AssignableFromData))]
    public void AssignableFrom(ReferenceCapabilityAssignmentTestCase row)
    {
        var isAssignable = row.To.IsAssignableFrom(row.From);

        Assert.Equal(row.Assignable, isAssignable);
    }

    public static TheoryData<ReferenceCapabilityAssignmentTestCase> AssignableFromData()
    {
        var data = new TheoryData<ReferenceCapabilityAssignmentTestCase>
        {
            // Up the standard hierarchy
            new ReferenceCapabilityAssignmentTestCase(Mutable, ReadOnly, true),
            new ReferenceCapabilityAssignmentTestCase(Constant, ReadOnly, true),
            new ReferenceCapabilityAssignmentTestCase(ReadOnly, Identity, true),
        };
        // All all transitive conversions?
        return data;
    }
}
