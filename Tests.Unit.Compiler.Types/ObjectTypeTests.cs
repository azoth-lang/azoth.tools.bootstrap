using Azoth.Tools.Bootstrap.Compiler.Types;
using Xunit;
using static Azoth.Tools.Bootstrap.Compiler.Types.ReferenceCapability;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types
{
    [Trait("Category", "Types")]
    public class ObjectTypeTests
    {
        [Fact]
        public void Has_reference_semantics()
        {
            var type = new ObjectType("Foo", "Bar", true, Isolated);

            Assert.Equal(TypeSemantics.Reference, type.Semantics);
        }

        [Fact]
        public void Convert_to_non_constant_type_is_same_type()
        {
            var type = new ObjectType("Foo", "Bar", true, Isolated);

            var nonConstant = type.ToNonConstantType();

            Assert.Same(type, nonConstant);
        }

        [Fact]
        public void With_same_name_mutability_and_reference_capability_are_equal()
        {
            var type1 = new ObjectType("Foo", "Bar", true, Isolated);
            var type2 = new ObjectType("Foo", "Bar", true, Isolated);

            Assert.Equal(type1, type2);
        }
    }
}
