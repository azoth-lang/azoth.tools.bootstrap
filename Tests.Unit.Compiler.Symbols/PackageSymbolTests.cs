using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Symbols
{
    [Trait("Category", "Symbols")]
    public class PackageSymbolTests : SymbolTestFixture
    {
        [Fact]
        public void Packages_with_same_name_are_equal()
        {
            var p1 = Package("some.package.name");
            var p2 = Package("some.package.name");

            Assert.Equal(p1, p2);
        }
    }
}
