using Azoth.Tools.Bootstrap.Framework;
using Azoth.Tools.Bootstrap.Tests.Unit.Framework.Fakes;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Framework;

[Trait("Category", "Framework")]
public class FixedSetTests
{
    [Fact]
    public void Equality_ignores_order()
    {
        var set1 = new[] { "foo", "bar", "baz" }.ToFixedSet();
        var set2 = new[] { "bar", "foo", "baz" }.ToFixedSet();

        // Use direct calls to Equals to avoid Xunit equals magic
        Assert.True(set1.Equals(set1));
        Assert.Equal(set1.GetHashCode(), set2.GetHashCode());
    }

    [Fact]
    public void EqualityComparer_ignores_order()
    {
        var set1 = new[] { "foo", "bar", "baz" }.ToFixedSet();
        var set2 = new[] { "bar", "foo", "baz" }.ToFixedSet();

        var comparer = FixedSet.EqualityComparer<string>();
        Assert.True(comparer.Equals(set1, set2));
        Assert.Equal(comparer.GetHashCode(set1), comparer.GetHashCode(set2));
    }

    [Fact]
    public void Can_compare_equality_of_sets_with_different_type()
    {
        var l1 = new[] { new FakeSquare(42), new FakeSquare(33), new FakeSquare(-98) }.ToFixedSet();
        var l2 = new[] { new FakeSquare(42), new FakeSquare(33), new FakeSquare(-98) }.ToFixedSet<FakeShape>();

        // Directly call `Equals` to ensure it is being used rather than some special collection
        // comparison of the unit test framework.
        Assert.True(l1.Equals(l2));
        Assert.True(l2.Equals(l1));
    }
}
