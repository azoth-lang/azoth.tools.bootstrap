using Xunit;
using static Azoth.Tools.Bootstrap.Compiler.Types.Capabilities.Capability;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types;

[Trait("Category", "Types")]
public class CapabilityTests
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
            new ReferenceCapabilityAssignmentTestCase(Mutable, Read, true),
            new ReferenceCapabilityAssignmentTestCase(Constant, Read, true),
            new ReferenceCapabilityAssignmentTestCase(Read, Identity, true),
            // Init
            new ReferenceCapabilityAssignmentTestCase(InitMutable, InitReadOnly, true),
            new ReferenceCapabilityAssignmentTestCase(InitMutable, Mutable, false),
            new ReferenceCapabilityAssignmentTestCase(Mutable, InitMutable, false),
            new ReferenceCapabilityAssignmentTestCase(InitReadOnly, Read, false),
            new ReferenceCapabilityAssignmentTestCase(Read, InitReadOnly, false),
            // Temp
            new ReferenceCapabilityAssignmentTestCase(TemporarilyIsolated, TemporarilyConstant, true),
            new ReferenceCapabilityAssignmentTestCase(Isolated, TemporarilyIsolated, true),
            new ReferenceCapabilityAssignmentTestCase(TemporarilyIsolated, Isolated, false),
            new ReferenceCapabilityAssignmentTestCase(Constant, TemporarilyConstant, true),
            new ReferenceCapabilityAssignmentTestCase(TemporarilyConstant, Constant, false),
            new ReferenceCapabilityAssignmentTestCase(TemporarilyConstant, Read, true),
        };
        // All all transitive conversions?
        return data;
    }
}
