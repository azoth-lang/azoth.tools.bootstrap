using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Languages;

internal static class LanguageParser
{
    public static Language ParseLanguage(string input)
    {
        var lines = Parsing.ParseLines(input).ToFixedList();

        var name = Parsing.GetConfig(lines, "name") ?? throw new FormatException("Language name is required");
        var ns = Parsing.GetConfig(lines, "namespace");
        var rootType = Parsing.ParseSymbol(Parsing.GetConfig(lines, "root"));
        var prefix = Parsing.GetConfig(lines, "prefix") ?? "";
        var suffix = Parsing.GetConfig(lines, "suffix") ?? "";
        var listType = Parsing.GetConfig(lines, "list") ?? "List";
        var usingNamespaces = Parsing.ParseUsingNamespaces(lines);
        var rules = Parsing.ParseRules(lines).Select(r => r.WithDefaultRootType(rootType));
        var grammar = new Grammar(ns, rootType, prefix, suffix, listType, usingNamespaces, rules);
        return new Language(name, grammar);
    }
}
