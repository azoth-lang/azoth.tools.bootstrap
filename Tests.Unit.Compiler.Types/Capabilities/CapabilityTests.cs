using Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types.TestCases;
using Xunit;
using static Azoth.Tools.Bootstrap.Compiler.Types.Capabilities.Capability;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types.Capabilities;

[Trait("Category", "Types")]
public class CapabilityTests
{
    [Theory]
    [MemberData(nameof(AssignableFromData))]
    public void AssignableFrom(CapabilityAssignmentTestCase row)
    {
        var isAssignable = row.To.IsAssignableFrom(row.From);

        Assert.Equal(row.Assignable, isAssignable);
    }

    public static TheoryData<CapabilityAssignmentTestCase> AssignableFromData()
    {
        var data = new TheoryData<CapabilityAssignmentTestCase>
        {
            // Up the standard hierarchy
            new CapabilityAssignmentTestCase(Isolated, Mutable, true),
            new CapabilityAssignmentTestCase(Isolated, Constant, true),
            new CapabilityAssignmentTestCase(Mutable, Read, true),
            new CapabilityAssignmentTestCase(Constant, Read, true),
            new CapabilityAssignmentTestCase(Read, Identity, true),
            // Init
            new CapabilityAssignmentTestCase(InitMutable, InitReadOnly, true),
            new CapabilityAssignmentTestCase(InitMutable, Mutable, false),
            new CapabilityAssignmentTestCase(Mutable, InitMutable, false),
            new CapabilityAssignmentTestCase(InitReadOnly, Read, false),
            new CapabilityAssignmentTestCase(Read, InitReadOnly, false),
            // Temp
            new CapabilityAssignmentTestCase(TemporarilyIsolated, TemporarilyConstant, true),
            new CapabilityAssignmentTestCase(Isolated, TemporarilyIsolated, true),
            new CapabilityAssignmentTestCase(TemporarilyIsolated, Isolated, false),
            new CapabilityAssignmentTestCase(Constant, TemporarilyConstant, true),
            new CapabilityAssignmentTestCase(TemporarilyConstant, Constant, false),
            new CapabilityAssignmentTestCase(TemporarilyConstant, Read, true),
        };
        // Add all transitive conversions?
        return data;
    }

    [Theory]
    [MemberData(nameof(AccessedViaData))]
    public void AccessedVia(AccessedViaTestCase row)
    {
        var effective = row.Member.AccessedVia(row.Object);

        Assert.Equal(row.Effective, effective);
    }

    public static TheoryData<AccessedViaTestCase> AccessedViaData() =>
    [
        // Object ▷ Member = Effective

        // TODO Currently not allowed, is that correct?
        /*new(TemporarilyIsolated, Isolated, Isolated),
        new(TemporarilyIsolated, TemporarilyIsolated, TemporarilyConstant),
        new(TemporarilyIsolated, Mutable, Mutable),
        new(TemporarilyIsolated, Read, Read),
        new(TemporarilyIsolated, Constant, Constant),
        new(TemporarilyIsolated, TemporarilyConstant, TemporarilyConstant),
        new(TemporarilyIsolated, Identity, Identity),*/

        // TODO I am not sure of the correct effective capability here
        //new(Mutable, Isolated, Mutable),
        //new(Mutable, TemporarilyIsolated, Mutable),
        new(Mutable, Mutable, Mutable),
        new(Mutable, Read, Read),
        new(Mutable, Constant, Constant),
        // TODO I am not sure of the correct effective capability here
        // new(Mutable, TemporarilyConstant, TemporarilyConstant),
        new(Mutable, Identity, Identity),

        new(Read, Isolated, Read),
        new(Read, TemporarilyIsolated, Read),
        new(Read, Mutable, Read),
        new(Read, Read, Read),
        new(Read, Constant, Constant),
        // TODO I am not sure of the correct effective capability here
        //new(Read, TemporarilyConstant, TemporarilyConstant),
        new(Read, Identity, Identity),

        new(TemporarilyConstant, Isolated, TemporarilyConstant),
        new(TemporarilyConstant, TemporarilyIsolated, TemporarilyConstant),
        new(TemporarilyConstant, Mutable, TemporarilyConstant),
        new(TemporarilyConstant, Read, TemporarilyConstant),
        new(TemporarilyConstant, Constant, Constant),
        new(TemporarilyConstant, TemporarilyConstant, TemporarilyConstant),
        new(TemporarilyConstant, Identity, Identity),

        new(Constant, Isolated, Constant),
        new(Constant, TemporarilyIsolated, Constant),
        new(Constant, Mutable, Constant),
        new(Constant, Read, Constant),
        new(Constant, Constant, Constant),
        new(Constant, TemporarilyConstant, Constant),
        new(Constant, Identity, Identity),

        new(Identity, Isolated, Identity),
        new(Identity, TemporarilyIsolated, Identity),
        new(Identity, Mutable, Identity),
        new(Identity, Read, Identity),
        new(Identity, Constant, Constant),
        new(Identity, TemporarilyConstant, Identity),
        new(Identity, Identity, Identity),
    ];
}
