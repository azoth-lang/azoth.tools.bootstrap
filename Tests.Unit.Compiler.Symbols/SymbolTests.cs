using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Moq;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Symbols
{
    [Trait("Category", "Symbols")]
    public class SymbolTests : SymbolTestFixture
    {
        [Fact]
        public void Symbol_not_in_namespace_is_global()
        {
            var symbol = FakeSymbol(null, Name("My_Class"));

            Assert.True(symbol.IsGlobal);
        }

        [Fact]
        public void Symbol_in_namespace_is_not_global()
        {
            var symbol = FakeSymbol(Namespace(), Name("My_Class"));

            Assert.False(symbol.IsGlobal);
        }

        private static Symbol FakeSymbol(NamespaceOrPackageSymbol? containing, Name name)
        {
            return new Mock<Symbol>(MockBehavior.Default, containing, name).Object;
        }
    }
}
