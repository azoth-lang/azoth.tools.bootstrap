using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees;

internal static class TreeParser
{
    public static TreeSyntax Parse(string treeDefinition)
    {
        var lines = Parsing.ParseLines(treeDefinition).ToFixedList();

        var ns = Parsing.GetRequiredConfig(lines, "namespace");
        var rootType = Parsing.ParseSymbol(Parsing.GetConfig(lines, "root"));
        var symbolPrefix = Parsing.GetConfig(lines, "prefix") ?? "";
        var symbolSuffix = Parsing.GetConfig(lines, "suffix") ?? "";
        var classPrefix = Parsing.GetConfig(lines, "class-prefix") ?? "";
        var classSuffix = Parsing.GetConfig(lines, "class-suffix") ?? "";
        var usingNamespaces = Parsing.ParseUsingNamespaces(lines);
        var rules = Parsing.ParseRules(lines);
        return new TreeSyntax(ns, rootType, symbolPrefix, symbolSuffix, classPrefix, classSuffix,
            usingNamespaces, rules);
    }
}
