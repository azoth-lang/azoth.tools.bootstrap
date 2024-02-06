using Xunit;
using static Azoth.Tools.Bootstrap.Compiler.Types.Capabilities.ReferenceCapability;

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
            new ReferenceCapabilityAssignmentTestCase(Isolated, Mutable, true),
            new ReferenceCapabilityAssignmentTestCase(Isolated, Constant, true),
            new ReferenceCapabilityAssignmentTestCase(Mutable, ReadOnly, true),
            new ReferenceCapabilityAssignmentTestCase(Constant, ReadOnly, true),
            new ReferenceCapabilityAssignmentTestCase(ReadOnly, Identity, true),
            // Init
            new ReferenceCapabilityAssignmentTestCase(InitMutable, InitReadOnly, true),
            new ReferenceCapabilityAssignmentTestCase(InitMutable, Mutable, false),
            new ReferenceCapabilityAssignmentTestCase(Mutable, InitMutable, false),
            new ReferenceCapabilityAssignmentTestCase(InitReadOnly, ReadOnly, false),
            new ReferenceCapabilityAssignmentTestCase(ReadOnly, InitReadOnly, false),
            // Temp
            new ReferenceCapabilityAssignmentTestCase(TemporarilyIsolated, TemporarilyConstant, true),
            new ReferenceCapabilityAssignmentTestCase(Isolated, TemporarilyIsolated, true),
            new ReferenceCapabilityAssignmentTestCase(TemporarilyIsolated, Isolated, false),
            new ReferenceCapabilityAssignmentTestCase(Constant, TemporarilyConstant, true),
            new ReferenceCapabilityAssignmentTestCase(TemporarilyConstant, Constant, false),
            new ReferenceCapabilityAssignmentTestCase(TemporarilyConstant, ReadOnly, true),
        };
        // All all transitive conversions?
        return data;
    }
}
