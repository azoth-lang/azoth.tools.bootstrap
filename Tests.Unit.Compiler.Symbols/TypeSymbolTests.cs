using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Symbols;

[Trait("Category", "Symbols")]
public class TypeSymbolTests : SymbolTestFixture
{
    [Fact]
    public void With_same_name_and_type_are_equal()
    {
        var package = Package("my.package");
        var facet = Facet(package);
        var type = DataType("T1");
        var sym1 = Type(facet, (OrdinaryTypeConstructor)type.PlainType.TypeConstructor!);
        var sym2 = Type(facet, (OrdinaryTypeConstructor)type.PlainType.TypeConstructor!);

        Assert.Equal(sym1, sym2);
    }

    [Fact]
    public void With_different_name_are_not_equal()
    {
        var package = Package("my.package");
        var facet = Facet(package);
        var type1 = DataType("My_Class1");
        var sym1 = Type(facet, (OrdinaryTypeConstructor)type1.PlainType.TypeConstructor!);
        var type2 = DataType("My_Class2");
        var sym2 = Type(facet, (OrdinaryTypeConstructor)type2.PlainType.TypeConstructor!);

        Assert.NotEqual(sym1, sym2);
    }

    [Fact]
    public void With_different_type_are_not_equal()
    {
        var package = Package("my.package");
        var facet = Facet(package);
        var type1 = DataType("T1");
        var sym1 = Type(facet, (OrdinaryTypeConstructor)type1.PlainType.TypeConstructor!);
        var type2 = DataType("T2");
        var sym2 = Type(facet, (OrdinaryTypeConstructor)type2.PlainType.TypeConstructor!);

        Assert.NotEqual(sym1, sym2);
    }
}
