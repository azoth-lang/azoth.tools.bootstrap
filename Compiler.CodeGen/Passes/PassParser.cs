using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Passes;

internal static class PassParser
{
    public static PassNode ParsePass(string input)
    {
        var lines = Parsing.ParseLines(input).ToFixedList();

        var name = Parsing.GetConfig(lines, "name") ?? throw new FormatException("Pass name is required");
        var fromName = Parsing.ParseSymbol(Parsing.GetConfig(lines, "from"));
        var toName = Parsing.ParseSymbol(Parsing.GetConfig(lines, "to"));
        var context = Parsing.GetConfig(lines, "context");
        var (fromContext, toContext) = ParseContext(context);

        return new PassNode(name, fromName, toName, fromContext, toContext);
    }

    private static (SymbolNode?, SymbolNode?) ParseContext(string? context)
    {
        if (context is null)
            return (null, null);

        var parts = context.Split("->");
        switch (parts.Length)
        {
            case 1:
                var contextSymbol = new SymbolNode(parts[0].Trim(), true);
                return (contextSymbol, contextSymbol);
            case 2:
                return (new SymbolNode(parts[0].Trim(), true), new SymbolNode(parts[1].Trim(), true));
            default:
                throw new FormatException($"Invalid context format '{context}'");
        }
    }
}
