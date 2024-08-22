using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees;
using Azoth.Tools.Bootstrap.Framework;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.CodeGen.Trees;

[Trait("Category", "CodeGen")]
public class TreeParserTests
{
    #region Options
    [Fact]
    public void DefaultsBaseTypeToNull()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;";

        var config = TreeParser.Parse(grammar);

        Assert.Null(config.Root);
    }

    [Fact]
    public void DefaultsPrefixToEmptyString()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;";

        var config = TreeParser.Parse(grammar);

        Assert.Equal("", config.SymbolPrefix);
    }

    [Fact]
    public void DefaultsSuffixToEmptyString()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;";

        var config = TreeParser.Parse(grammar);

        Assert.Equal("", config.SymbolSuffix);
    }

    [Fact]
    public void ParsesNamespace()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;";

        var config = TreeParser.Parse(grammar);

        Assert.Equal("Foo.Bar.Baz", config.Namespace);
    }

    [Fact]
    public void ParsesRootSupertype()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\r◊root-supertype MyBase;";

        var config = TreeParser.Parse(grammar);

        Assert.Equal(Symbol("MyBase"), config.Root);
    }

    [Fact]
    public void ParsesQuotedRootSupertype()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\r◊root-supertype `MyBase`;";

        var config = TreeParser.Parse(grammar);

        Assert.Equal(QuotedSymbol("MyBase"), config.Root);
    }

    [Fact]
    public void ParsesPrefix()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\r◊prefix MyPrefix;";

        var config = TreeParser.Parse(grammar);

        Assert.Equal("MyPrefix", config.SymbolPrefix);
    }

    [Fact]
    public void ParsesSuffix()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\r◊suffix MySuffix;";

        var config = TreeParser.Parse(grammar);

        Assert.Equal("MySuffix", config.SymbolSuffix);
    }

    [Fact]
    public void ParsesUsingNamespaces()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\r◊using Foo.Bar;\r◊using Foo.Bar.Baz;";

        var config = TreeParser.Parse(grammar);

        Assert.Equal(FixedList("Foo.Bar", "Foo.Bar.Baz"), config.UsingNamespaces);
    }
    #endregion

    #region Nodes
    [Fact]
    public void ParsesSimpleTerminalRule()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\rSubType;";
        var config = TreeParser.Parse(grammar);

        var rule = Assert.Single(config.Nodes);
        Assert.Equal(Symbol("SubType"), rule.Defines);
        Assert.Empty(rule.Supertypes);
        Assert.Empty(rule.DeclaredProperties);
    }

    [Fact]
    public void ParsesSimpleQuotedTerminalRule()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\r`IMyFullTypeName`;";
        var config = TreeParser.Parse(grammar);

        var rule = Assert.Single(config.Nodes);
        Assert.Equal(QuotedSymbol("IMyFullTypeName"), rule.Defines);
        Assert.Empty(rule.Supertypes);
        Assert.Empty(rule.DeclaredProperties);
    }

    [Fact]
    public void ParsesSimpleTerminalRuleWithRootType()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\r◊root MyBase;\nSubType;";
        var config = TreeParser.Parse(grammar);

        var rule = Assert.Single(config.Nodes);
        Assert.Equal(Symbol("SubType"), rule.Defines);
        var expectedParents = FixedList<SymbolSyntax>();
        Assert.Equal(expectedParents, rule.Supertypes);
        Assert.Empty(rule.DeclaredProperties);
    }

    [Fact]
    public void ParsesBaseTypeRule()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\r◊base MyBase;\nMyBase;";
        var config = TreeParser.Parse(grammar);

        var rule = Assert.Single(config.Nodes);
        Assert.Equal(Symbol("MyBase"), rule.Defines);
        Assert.Empty(rule.Supertypes);
        Assert.Empty(rule.DeclaredProperties);
    }

    [Fact]
    public void ParsesSingleInheritanceRule()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\rSubType <: BaseType;";

        var config = TreeParser.Parse(grammar);

        var rule = Assert.Single(config.Nodes);
        Assert.Equal(Symbol("SubType"), rule.Defines);
        Assert.Single(rule.Supertypes, Symbol("BaseType"));
        Assert.Empty(rule.DeclaredProperties);
    }

    [Fact]
    public void ParsesMultipleInheritanceRule()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\rSubType <: BaseType1, BaseType2;";

        var config = TreeParser.Parse(grammar);

        var rule = Assert.Single(config.Nodes);
        Assert.Equal(Symbol("SubType"), rule.Defines);
        var expectedParents = FixedList(Symbol("BaseType1"), Symbol("BaseType2"));
        Assert.Equal(expectedParents, rule.Supertypes);
        Assert.Empty(rule.DeclaredProperties);
    }

    [Fact]
    public void ParseQuotedInheritanceRule()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\rSubType <: `BaseType1`, BaseType2;";

        var config = TreeParser.Parse(grammar);

        var rule = Assert.Single(config.Nodes);
        Assert.Equal(new SymbolSyntax("SubType"), rule.Defines);
        var expectedParents = FixedSet(QuotedSymbol("BaseType1"), Symbol("BaseType2"));
        Assert.Equal(expectedParents, rule.Supertypes);
        Assert.Empty(rule.DeclaredProperties);
    }

    [Fact]
    public void ParseErrorTooManyEqualSigns()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\rNonTerminal = Foo = Bar;";

        var ex = Assert.Throws<FormatException>(() => TreeParser.Parse(grammar));

        Assert.Equal("Too many equal signs in: 'NonTerminal = Foo = Bar'", ex.Message);
    }
    #endregion

    #region Comments
    [Fact]
    public void Ignores_line_comments()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\r// A comment";
        var config = TreeParser.Parse(grammar);

        Assert.Empty(config.Nodes);
    }
    #endregion

    #region Properties
    [Fact]
    public void ParsesSimpleProperty()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\rMyNonterminal = MyProperty;";
        var config = TreeParser.Parse(grammar);

        var rule = Assert.Single(config.Nodes);
        var property = Assert.Single(rule.DeclaredProperties);
        Assert.Equal("MyProperty", property.Name);
        Assert.Equal(SymbolType(Symbol("MyProperty")), property.Type);
    }

    [Fact]
    public void ParsesSimpleOptionalProperty()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\rMyNonterminal = MyProperty?;";
        var config = TreeParser.Parse(grammar);

        var rule = Assert.Single(config.Nodes);
        var property = Assert.Single(rule.DeclaredProperties);
        Assert.Equal("MyProperty", property.Name);
        Assert.Equal(OptionalType(SymbolType("MyProperty")), property.Type);
    }

    [Fact]
    public void ParsesTypedProperty()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\rMyNonterminal = MyProperty:MyType;";
        var config = TreeParser.Parse(grammar);

        var rule = Assert.Single(config.Nodes);
        var property = Assert.Single(rule.DeclaredProperties);
        Assert.Equal("MyProperty", property.Name);
        Assert.Equal(SymbolType(Symbol("MyType")), property.Type);
    }

    [Fact]
    public void ParsesQuotedTypedProperty()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\rMyNonterminal = MyProperty:`MyType`;";
        var config = TreeParser.Parse(grammar);

        var rule = Assert.Single(config.Nodes);
        var property = Assert.Single(rule.DeclaredProperties);
        Assert.Equal("MyProperty", property.Name);
        Assert.Equal(SymbolType(QuotedSymbol("MyType")), property.Type);
    }

    [Fact]
    public void ParsesListTypedProperty()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\rMyNonterminal = MyProperty:MyType*;";
        var config = TreeParser.Parse(grammar);

        var rule = Assert.Single(config.Nodes);
        var property = Assert.Single(rule.DeclaredProperties);
        Assert.Equal("MyProperty", property.Name);
        Assert.Equal(ListType(SymbolType("MyType")), property.Type);
    }

    [Fact]
    public void ParsesOptionalTypedProperty()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\rMyNonterminal = MyProperty:MyType?;";
        var config = TreeParser.Parse(grammar);

        var rule = Assert.Single(config.Nodes);
        var property = Assert.Single(rule.DeclaredProperties);
        Assert.Equal("MyProperty", property.Name);
        Assert.Equal(OptionalType(SymbolType("MyType")), property.Type);
    }

    [Fact]
    public void ParsesListOfOptionalTypedProperty()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\rMyNonterminal = MyProperty:MyType?*;";
        var config = TreeParser.Parse(grammar);

        var rule = Assert.Single(config.Nodes);
        var property = Assert.Single(rule.DeclaredProperties);
        Assert.Equal("MyProperty", property.Name);
        Assert.Equal(ListType(OptionalType(SymbolType("MyType"))), property.Type);
    }

    [Fact]
    public void ParsesOptionalListTypedProperty()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\rMyNonterminal = MyProperty:MyType*?;";
        var config = TreeParser.Parse(grammar);

        var rule = Assert.Single(config.Nodes);
        var property = Assert.Single(rule.DeclaredProperties);
        Assert.Equal("MyProperty", property.Name);
        Assert.Equal(OptionalType(ListType(SymbolType("MyType"))), property.Type);
    }

    [Fact]
    public void ParseErrorTooManyColonsInDefinition()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\rMyNonterminal = MyProperty:MyType:What;";

        var ex = Assert.Throws<FormatException>(() => TreeParser.Parse(grammar));

        Assert.Equal("Too many colons in binding: 'MyProperty:MyType:What'", ex.Message);
    }

    [Fact]
    public void ParsesMultipleProperties()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\rMyNonterminal = MyProperty1:MyType1 MyProperty2:MyType2*;";
        var config = TreeParser.Parse(grammar);

        var rule = Assert.Single(config.Nodes);
        Assert.Collection(rule.DeclaredProperties, p1 =>
        {
            Assert.Equal("MyProperty1", p1.Name);
            Assert.Equal(SymbolType(Symbol("MyType1")), p1.Type);
        }, p2 =>
        {
            Assert.Equal("MyProperty2", p2.Name);
            Assert.Equal(ListType(SymbolType("MyType2")), p2.Type);
        });
    }

    [Fact]
    public void ParseErrorDuplicateProperties()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;\rMyNonterminal = Something Something:'Blah';";

        var ex = Assert.Throws<ArgumentException>(() => TreeParser.Parse(grammar));

        Assert.Equal("Node MyNonterminal contains duplicate property definitions.", ex.Message);
    }
    #endregion

    private static SymbolSyntax Symbol(string text) => new(text);

    private static SymbolSyntax QuotedSymbol(string text) => new(text, true);

    private static SymbolTypeSyntax SymbolType(string text) => new(Symbol(text));

    private static SymbolTypeSyntax SymbolType(SymbolSyntax symbol) => new(symbol);

    private static OptionalTypeSyntax OptionalType(TypeSyntax type) => new(type);

    private static CollectionTypeSyntax ListType(TypeSyntax type) => new(CollectionKind.List, type);

    private static IFixedList<T> FixedList<T>(params T[] values) => values.ToFixedList();

    private static IFixedSet<T> FixedSet<T>(params T[] values) => values.ToFixedSet();
}
