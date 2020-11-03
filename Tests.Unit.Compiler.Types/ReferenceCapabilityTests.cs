using Xunit;
using static Azoth.Tools.Bootstrap.Compiler.Types.ReferenceCapability;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types
{
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
                // From something to the lent version
                new ReferenceCapabilityAssignmentTestCase(Isolated, LentIsolated, true),
                new ReferenceCapabilityAssignmentTestCase(Transition, LentTransition, true),
                new ReferenceCapabilityAssignmentTestCase(SharedMutable, LentMutable, true),
                new ReferenceCapabilityAssignmentTestCase(Const, LentConst, true),
                new ReferenceCapabilityAssignmentTestCase(Shared, Lent, true),
                new ReferenceCapabilityAssignmentTestCase(Identity, LentIdentity, true),

                // Up the standard hierarchy
                new ReferenceCapabilityAssignmentTestCase(Isolated, Transition, true),
                new ReferenceCapabilityAssignmentTestCase(Transition, SharedMutable, true),
                new ReferenceCapabilityAssignmentTestCase(Transition, Const, true),
                new ReferenceCapabilityAssignmentTestCase(SharedMutable, Shared, true),
                new ReferenceCapabilityAssignmentTestCase(Const, Shared, true),
                new ReferenceCapabilityAssignmentTestCase(Shared, Identity, true),

                // Up the lent hierarchy
                new ReferenceCapabilityAssignmentTestCase(LentIsolated, LentTransition, true),
                new ReferenceCapabilityAssignmentTestCase(LentTransition, LentMutable, true),
                new ReferenceCapabilityAssignmentTestCase(LentTransition, LentConst, true),
                new ReferenceCapabilityAssignmentTestCase(LentMutable, Lent, true),
                new ReferenceCapabilityAssignmentTestCase(LentConst, Lent, true),
                new ReferenceCapabilityAssignmentTestCase(Lent, LentIdentity, true),
            };
            // All all transitive conversions?
            return data;
        }
    }
}
