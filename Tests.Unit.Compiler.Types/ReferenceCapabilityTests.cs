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
                new ReferenceCapabilityAssignmentTestCase(SharedMutable, LentMutable, true),
                new ReferenceCapabilityAssignmentTestCase(Shared, Lent, true),

                // Up the standard hierarchy
                new ReferenceCapabilityAssignmentTestCase(SharedMutable, Shared, true),
                new ReferenceCapabilityAssignmentTestCase(Constant, Shared, true),
                new ReferenceCapabilityAssignmentTestCase(Shared, Identity, true),

                // Up the lent hierarchy
                new ReferenceCapabilityAssignmentTestCase(LentMutable, Lent, true),
            };
            // All all transitive conversions?
            return data;
        }
    }
}
