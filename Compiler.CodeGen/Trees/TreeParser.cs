using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees;

internal static class TreeParser
{
    public static TreeSyntax Parse(string grammar)
    {
        var lines = Parsing.ParseLines(grammar).ToFixedList();

        var ns = Parsing.GetRequiredConfig(lines, "namespace");
        var rootType = Parsing.ParseSymbol(Parsing.GetConfig(lines, "root"));
        var prefix = Parsing.GetConfig(lines, "prefix") ?? "";
        var suffix = Parsing.GetConfig(lines, "suffix") ?? "";
        var usingNamespaces = Parsing.ParseUsingNamespaces(lines);
        var rules = Parsing.ParseRules(lines);
        return new TreeSyntax(ns, rootType, prefix, suffix, usingNamespaces, rules);
    }
}
