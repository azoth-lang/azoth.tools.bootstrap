using Azoth.Tools.Bootstrap.Framework;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Framework
{
    [Trait("Category", "Framework")]
    public class FixedSetTests
    {
        [Fact]
        public void Equal_fixed_sets_have_same_hash_code()
        {
            var set1 = new[] { "foo", "bar", "baz" }.ToFixedSet();
            var set2 = new[] { "foo", "bar", "baz" }.ToFixedSet();

            Assert.Equal(set1, set2);
            Assert.Equal(set1.GetHashCode(), set2.GetHashCode());
        }
    }
}
