using Azoth.Tools.Bootstrap.Framework;
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

        Assert.True(l1.ItemsEquals(l2));
    }

    [Fact]
    public void Equal_fixed_lists_are_equal_and_have_same_hash_code()
    {
        var l1 = new[] { "foo", "bar", "baz" }.ToFixedList();
        var l2 = new[] { "foo", "bar", "baz" }.ToFixedList();

        Assert.Equal(l1.GetHashCode(), l2.GetHashCode());
    }
}
