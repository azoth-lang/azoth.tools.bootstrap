using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees;
using Azoth.Tools.Bootstrap.Framework;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.CodeGen.Trees;

[Trait("Category", "CodeGen")]
public class TreeParserTests
{
    #region Options
    [Fact]
    public void DefaultsNamespaceToNull()
    {
        const string grammar = "";

        var config = TreeParser.ParseGrammar(grammar);

        Assert.Null(config.Namespace);
    }

    [Fact]
    public void DefaultsBaseTypeToNull()
    {
        const string grammar = "";

        var config = TreeParser.ParseGrammar(grammar);

        Assert.Null(config.DefaultParent);
    }

    [Fact]
    public void DefaultsPrefixToEmptyString()
    {
        const string grammar = "";

        var config = TreeParser.ParseGrammar(grammar);

        Assert.Equal("", config.Prefix);
    }

    [Fact]
    public void DefaultsSuffixToEmptyString()
    {
        const string grammar = "";

        var config = TreeParser.ParseGrammar(grammar);

        Assert.Equal("", config.Suffix);
    }

    [Fact]
    public void DefaultsListTypeToList()
    {
        const string grammar = "";

        var config = TreeParser.ParseGrammar(grammar);

        Assert.Equal("List", config.ListType);
    }

    [Fact]
    public void ParsesNamespace()
    {
        const string grammar = "◊namespace Foo.Bar.Baz;";

        var config = TreeParser.ParseGrammar(grammar);

        Assert.Equal("Foo.Bar.Baz", config.Namespace);
    }

    [Fact]
    public void ParsesRootType()
    {
        const string grammar = "◊root MyBase;";

        var config = TreeParser.ParseGrammar(grammar);

        Assert.Equal(Symbol("MyBase"), config.DefaultParent);
    }

    [Fact]
    public void ParsesQuotedRootType()
    {
        const string grammar = "◊root `MyBase`;";

        var config = TreeParser.ParseGrammar(grammar);

        Assert.Equal(QuotedSymbol("MyBase"), config.DefaultParent);
    }

    [Fact]
    public void ParsesPrefix()
    {
        const string grammar = "◊prefix MyPrefix;";

        var config = TreeParser.ParseGrammar(grammar);

        Assert.Equal("MyPrefix", config.Prefix);
    }

    [Fact]
    public void ParsesSuffix()
    {
        const string grammar = "◊suffix MySuffix;";

        var config = TreeParser.ParseGrammar(grammar);

        Assert.Equal("MySuffix", config.Suffix);
    }

    [Fact]
    public void ParsesListType()
    {
        const string grammar = "◊list MyList;";

        var config = TreeParser.ParseGrammar(grammar);

        Assert.Equal("MyList", config.ListType);
    }

    [Fact]
    public void ParsesUsingNamespaces()
    {
        const string grammar = "◊using Foo.Bar;\r◊using Foo.Bar.Baz;";

        var config = TreeParser.ParseGrammar(grammar);

        Assert.Equal(FixedList("Foo.Bar", "Foo.Bar.Baz"), config.UsingNamespaces);
    }
    #endregion

    #region Rules
    [Fact]
    public void ParsesSimpleTerminalRule()
    {
        const string grammar = "SubType;";
        var config = TreeParser.ParseGrammar(grammar);

        var rule = Assert.Single(config.Rules);
        Assert.Equal(Symbol("SubType"), rule.Defines);
        Assert.Empty(rule.Supertypes);
        Assert.Empty(rule.DeclaredProperties);
    }

    [Fact]
    public void ParsesSimpleQuotedTerminalRule()
    {
        const string grammar = "`IMyFullTypeName`;";
        var config = TreeParser.ParseGrammar(grammar);

        var rule = Assert.Single(config.Rules);
        Assert.Equal(QuotedSymbol("IMyFullTypeName"), rule.Defines);
        Assert.Empty(rule.Supertypes);
        Assert.Empty(rule.DeclaredProperties);
    }

    [Fact]
    public void ParsesSimpleTerminalRuleWithRootType()
    {
        const string grammar = "◊root MyBase;\nSubType;";
        var config = TreeParser.ParseGrammar(grammar);

        var rule = Assert.Single(config.Rules);
        Assert.Equal(Symbol("SubType"), rule.Defines);
        var expectedParents = FixedList(Symbol("MyBase"));
        Assert.Equal(expectedParents, rule.Parents);
        Assert.Empty(rule.DeclaredProperties);
    }

    [Fact]
    public void ParsesBaseTypeRule()
    {
        const string grammar = "◊base MyBase;\nMyBase;";
        var config = TreeParser.ParseGrammar(grammar);

        var rule = Assert.Single(config.Rules);
        Assert.Equal(Symbol("MyBase"), rule.Defines);
        Assert.Empty(rule.Supertypes);
        Assert.Empty(rule.DeclaredProperties);
    }

    [Fact]
    public void ParsesSingleInheritanceRule()
    {
        const string grammar = "SubType: BaseType;";

        var config = TreeParser.ParseGrammar(grammar);

        var rule = Assert.Single(config.Rules);
        Assert.Equal(Symbol("SubType"), rule.Defines);
        Assert.Single(rule.Parents, Symbol("BaseType"));
        Assert.Empty(rule.DeclaredProperties);
    }

    [Fact]
    public void ParsesMultipleInheritanceRule()
    {
        const string grammar = "SubType <: BaseType1, BaseType2;";

        var config = TreeParser.ParseGrammar(grammar);

        var rule = Assert.Single(config.Rules);
        Assert.Equal(Symbol("SubType"), rule.Defines);
        var expectedParents = FixedList(Symbol("BaseType1"), Symbol("BaseType2"));
        Assert.Equal(expectedParents, rule.Supertypes);
        Assert.Empty(rule.DeclaredProperties);
    }

    [Fact]
    public void ParseQuotedInheritanceRule()
    {
        const string grammar = "SubType: `BaseType1` <: BaseType2;";

        var config = TreeParser.ParseGrammar(grammar);

        var rule = Assert.Single(config.Rules);
        Assert.Equal(new SymbolNode("SubType"), rule.Defines);
        var expectedParents = FixedList(QuotedSymbol("BaseType1"), Symbol("BaseType2"));
        Assert.Equal(expectedParents, rule.Parents);
        Assert.Empty(rule.DeclaredProperties);
    }

    [Fact]
    public void ParseErrorTooManyEqualSigns()
    {
        const string grammar = "NonTerminal = Foo = Bar;";

        var ex = Assert.Throws<FormatException>(() => TreeParser.ParseGrammar(grammar));

        Assert.Equal("Too many equal signs on line: 'NonTerminal = Foo = Bar'", ex.Message);
    }

    [Fact]
    public void ParseErrorTooManyColonsInDeclaration()
    {
        const string grammar = "SubType: BaseType: What = Foo;";

        var ex = Assert.Throws<FormatException>(() => TreeParser.ParseGrammar(grammar));

        Assert.Equal("Too many colons in declaration: 'SubType: BaseType: What'", ex.Message);
    }
    #endregion

    #region Comments
    [Fact]
    public void Ignores_line_comments()
    {
        const string grammar = "// A comment";
        var config = TreeParser.ParseGrammar(grammar);

        Assert.Empty(config.Rules);
    }
    #endregion

    #region Properties
    [Fact]
    public void ParsesSimpleProperty()
    {
        const string grammar = "MyNonterminal = MyProperty;";
        var config = TreeParser.ParseGrammar(grammar);

        var rule = Assert.Single(config.Rules);
        var property = Assert.Single(rule.DeclaredProperties);
        Assert.Equal("MyProperty", property.Name);
        Assert.Equal(Type(Symbol("MyProperty")), property.Type);
    }

    [Fact]
    public void ParsesSimpleOptionalProperty()
    {
        const string grammar = "MyNonterminal = MyProperty?;";
        var config = TreeParser.ParseGrammar(grammar);

        var rule = Assert.Single(config.Rules);
        var property = Assert.Single(rule.DeclaredProperties);
        Assert.Equal("MyProperty", property.Name);
        Assert.Equal(OptionalType(Symbol("MyProperty")), property.Type);
    }

    [Fact]
    public void ParsesTypedProperty()
    {
        const string grammar = "MyNonterminal = MyProperty:MyType;";
        var config = TreeParser.ParseGrammar(grammar);

        var rule = Assert.Single(config.Rules);
        var property = Assert.Single(rule.DeclaredProperties);
        Assert.Equal("MyProperty", property.Name);
        Assert.Equal(Type(Symbol("MyType")), property.Type);
    }

    [Fact]
    public void ParsesQuotedTypedProperty()
    {
        const string grammar = "MyNonterminal = MyProperty:`MyType`;";
        var config = TreeParser.ParseGrammar(grammar);

        var rule = Assert.Single(config.Rules);
        var property = Assert.Single(rule.DeclaredProperties);
        Assert.Equal("MyProperty", property.Name);
        Assert.Equal(Type(QuotedSymbol("MyType")), property.Type);
    }

    [Fact]
    public void ParsesListTypedProperty()
    {
        const string grammar = "MyNonterminal = MyProperty:MyType*;";
        var config = TreeParser.ParseGrammar(grammar);

        var rule = Assert.Single(config.Rules);
        var property = Assert.Single(rule.DeclaredProperties);
        Assert.Equal("MyProperty", property.Name);
        Assert.Equal(ListType(Symbol("MyType")), property.Type);
    }

    [Fact]
    public void ParsesOptionalTypedProperty()
    {
        const string grammar = "MyNonterminal = MyProperty:MyType?;";
        var config = TreeParser.ParseGrammar(grammar);

        var rule = Assert.Single(config.Rules);
        var property = Assert.Single(rule.DeclaredProperties);
        Assert.Equal("MyProperty", property.Name);
        Assert.Equal(OptionalType(Symbol("MyType")), property.Type);
    }

    [Fact]
    public void ParsesListOfOptionalTypedProperty()
    {
        const string grammar = "MyNonterminal = MyProperty:MyType*?;";
        var config = TreeParser.ParseGrammar(grammar);

        var rule = Assert.Single(config.Rules);
        var property = Assert.Single(rule.DeclaredProperties);
        Assert.Equal("MyProperty", property.Name);
        Assert.Equal(new TypeNode(Symbol("MyType"), true, true), property.Type);
    }

    [Fact]
    public void ParseErrorTooManyColonsInDefinition()
    {
        const string grammar = "MyNonterminal = MyProperty:MyType:What;";

        var ex = Assert.Throws<FormatException>(() => TreeParser.ParseGrammar(grammar));

        Assert.Equal("Too many colons in property: 'MyProperty:MyType:What'", ex.Message);
    }

    [Fact]
    public void ParsesMultipleProperties()
    {
        const string grammar = "MyNonterminal = MyProperty1:MyType1 MyProperty2:MyType2*;";
        var config = TreeParser.ParseGrammar(grammar);

        var rule = Assert.Single(config.Rules);
        Assert.Collection(rule.DeclaredProperties, p1 =>
        {
            Assert.Equal("MyProperty1", p1.Name);
            Assert.Equal(Type(Symbol("MyType1")), p1.Type);
        }, p2 =>
        {
            Assert.Equal("MyProperty2", p2.Name);
            Assert.Equal(ListType(Symbol("MyType2")), p2.Type);
        });
    }

    [Fact]
    public void ParseErrorDuplicateProperties()
    {
        const string grammar = "MyNonterminal = Something Something:'Blah';";

        var ex = Assert.Throws<ArgumentException>(() => TreeParser.ParseGrammar(grammar));

        Assert.Equal("Rule for MyNonterminal contains duplicate property definitions", ex.Message);
    }
    #endregion

    private static SymbolNode Symbol(string text)
    {
        return new SymbolNode(text);
    }

    private static SymbolNode QuotedSymbol(string text)
    {
        return new SymbolNode(text, true);
    }

    private static TypeNode Type(SymbolNode symbol)
    {
        return new TypeNode(symbol, false, false);
    }

    private static TypeNode OptionalType(SymbolNode symbol)
    {
        return new TypeNode(symbol, false, true);
    }

    private static TypeNode ListType(SymbolNode symbol)
    {
        return new TypeNode(symbol, true, false);
    }

    private static TypeNode RefType(SymbolNode symbol)
    {
        return new TypeNode(symbol, false, false);
    }

    private static IFixedList<T> FixedList<T>(params T[] values)
    {
        return Framework.FixedList.Create(values);
    }
}
