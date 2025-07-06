using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Symbols;

[Trait("Category", "Symbols")]
public class NamespaceSymbolTests : SymbolTestFixture
{
    [Fact]
    public void Namespaces_with_same_name_and_containing_symbol_are_equal()
    {
        var package = Package("my.something_package");
        var facet = Facet(package);
        var nsFoo1 = Namespace("foo", facet);
        var nsFoo2 = Namespace("foo", facet);

        Assert.Equal(nsFoo1, nsFoo2);
    }

    [Fact]
    public void Namespaces_with_different_name_are_not_equal()
    {
        var package = Package("my.something_package");
        var facet = Facet(package);
        var nsFoo = Namespace("foo", facet);
        var nsBar = Namespace("bar", facet);

        Assert.NotEqual(nsFoo, nsBar);
    }

    [Fact]
    public void Namespaces_with_different_containing_symbols_are_not_equal()
    {
        var package1 = Package("my.something_package");
        var facet1 = Facet(package1);
        var ns1 = Namespace("foo", facet1);
        var package2 = Package("my.other_package");
        var facet2 = Facet(package2);
        var ns2 = Namespace("foo", facet2);

        Assert.NotEqual(ns1, ns2);
    }
}
