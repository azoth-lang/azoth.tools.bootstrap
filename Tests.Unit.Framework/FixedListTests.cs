using Azoth.Tools.Bootstrap.Framework;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Framework
{
    [Trait("Category", "Framework")]
    public class FixedListTests
    {
        [Fact]
        public void Equal_fixed_lists_have_same_hash_code()
        {
            var l1 = new[] { "foo", "bar", "baz" }.ToFixedList();
            var l2 = new[] { "foo", "bar", "baz" }.ToFixedList();

            Assert.Equal(l1, l2);
            Assert.Equal(l1.GetHashCode(), l2.GetHashCode());
        }
    }
}
