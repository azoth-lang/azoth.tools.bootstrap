using Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types.TestCases;
using Xunit;
using static Azoth.Tools.Bootstrap.Compiler.Types.Capabilities.Capability;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types.Capabilities;

[Trait("Category", "Types")]
public class CapabilityTests
{
    [Theory]
    [MemberData(nameof(AssignableFromData))]
    public void IsSubtypeOf(CapabilitySubtypeTestCase row)
    {
        var isSubtype = row.Left.IsSubtypeOf(row.Right);

        Assert.Equal(row.IsSubtype, isSubtype);
    }

    public static TheoryData<CapabilitySubtypeTestCase> AssignableFromData()
    {
        var data = new TheoryData<CapabilitySubtypeTestCase>
        {
            // Up the standard hierarchy
            new(Isolated, Mutable, true),
            new(Isolated, Constant, true),
            new(Mutable, Read, true),
            new(Constant, Read, true),
            new(Read, Identity, true),
            // Init
            new(InitMutable, InitRead, true),
            new(InitMutable, Mutable, false),
            new(Mutable, InitMutable, false),
            new(InitRead, Read, false),
            new(Read, InitRead, false),
            // Temp
            new(TemporarilyIsolated, TemporarilyConstant, true),
            new(Isolated, TemporarilyIsolated, true),
            new(TemporarilyIsolated, Mutable, true),
            new(TemporarilyIsolated, Isolated, false),
            new(Constant, TemporarilyConstant, true),
            new(TemporarilyConstant, Constant, false),
            new(TemporarilyConstant, Read, true),
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
