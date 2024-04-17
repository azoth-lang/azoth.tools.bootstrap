using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees;

internal static class TreeParser
{
    public static GrammarNode ParseGrammar(string grammar)
    {
        var lines = Parsing.ParseLines(grammar).ToFixedList();

        var ns = Parsing.GetRequiredConfig(lines, "namespace");
        var rootType = Parsing.ParseSymbol(Parsing.GetConfig(lines, "root"));
        var prefix = Parsing.GetConfig(lines, "prefix") ?? "";
        var suffix = Parsing.GetConfig(lines, "suffix") ?? "";
        var listType = Parsing.GetListConfig(lines);
        var setType = Parsing.GetSetConfig(lines);
        var usingNamespaces = Parsing.ParseUsingNamespaces(lines);
        var rules = Parsing.ParseRules(lines, rootType);
        return new GrammarNode(ns, rootType, prefix, suffix, listType, setType, usingNamespaces, rules);
    }
}
