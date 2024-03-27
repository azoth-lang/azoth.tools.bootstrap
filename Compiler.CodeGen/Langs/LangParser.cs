using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Langs;

internal static class LangParser
{
    public static Grammar ParseGrammar(string grammar)
    {
        var lines = Parsing.ParseLines(grammar).ToFixedList();

        var ns = Parsing.GetConfig(lines, "namespace");
        var rootType = Parsing.ParseSymbol(Parsing.GetConfig(lines, "root"));
        var prefix = Parsing.GetConfig(lines, "prefix") ?? "";
        var suffix = Parsing.GetConfig(lines, "suffix") ?? "";
        var listType = Parsing.GetConfig(lines, "list") ?? "List";
        var usingNamespaces = Parsing.GetUsingNamespaces(lines);
        var rules = Parsing.ParseRules(lines).Select(r => r.WithDefaultRootType(rootType));
        return new Grammar(ns, rootType, prefix, suffix, listType, usingNamespaces, rules);
    }
}
