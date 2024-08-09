using Azoth.Tools.Bootstrap.Framework;
using Azoth.Tools.Bootstrap.Tests.Unit.Framework.Fakes;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Framework;

[Trait("Category", "Framework")]
public class FixedListTests
{
    [Fact]
    public void Equal_fixed_lists_are_items_equal()
    {
        var l1 = new[] { "foo", "bar", "baz" }.ToFixedList();
        var l2 = new[] { "foo", "bar", "baz" }.ToFixedList();

        Assert.True(l1.Equals(l2));
    }

    [Fact]
    public void Equal_fixed_lists_are_equal_and_have_same_hash_code()
    {
        var l1 = new[] { "foo", "bar", "baz" }.ToFixedList();
        var l2 = new[] { "foo", "bar", "baz" }.ToFixedList();

        Assert.Equal(l1.GetHashCode(), l2.GetHashCode());
    }

    [Fact]
    public void Can_compare_equality_of_lists_with_different_type()
    {
        var l1 = new[] { new FakeSquare(42), new FakeSquare(33), new FakeSquare(-98) }.ToFixedList();
        var l2 = new[] { new FakeSquare(42), new FakeSquare(33), new FakeSquare(-98) }.ToFixedList<FakeShape>();

        // Directly call `Equals` to ensure it is being used rather than some special collection
        // comparison of the unit test framework.
        Assert.True(l1.Equals(l2));
        Assert.True(l2.Equals(l1));
    }
}
