using Azoth.Tools.Bootstrap.Framework;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Framework;

[Trait("Category", "Framework")]
public class FixedSetTests
{
    [Fact]
    public void Has_reference_equality()
    {
        var set1 = new[] { "foo", "bar", "baz" }.ToFixedSet();
        var set2 = new[] { "bar", "foo", "baz" }.ToFixedSet();

        // Use direct calls to Equals to avoid Xunit equals magic
        Assert.True(set1.Equals(set1));
        Assert.False(set1.Equals(set2));
        Assert.NotEqual(set1.GetHashCode(), set2.GetHashCode());
    }

    [Fact]
    public void Item_comparer_compares_by_items_regardless_of_order()
    {
        var set1 = new[] { "foo", "bar", "baz" }.ToFixedSet();
        var set2 = new[] { "bar", "foo", "baz" }.ToFixedSet();

        var comparer = FixedSet.ItemComparer<string>();
        Assert.True(comparer.Equals(set1, set2));
        Assert.Equal(comparer.GetHashCode(set1), comparer.GetHashCode(set2));
    }
}
