namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed class LanguageNode
{
    public string? Name { get; }
    public string DefinitionFilePath { get; }
    public SymbolNode Entry { get; }
    public GrammarNode Grammar { get; }
    public LanguageNode? Extends { get; }

    public LanguageNode(
        string? name,
        string definitionFilePath,
        SymbolNode entry,
        GrammarNode grammar,
        LanguageNode? extends)
    {
        Name = name;
        DefinitionFilePath = definitionFilePath;
        Grammar = grammar;
        Extends = extends;
        Entry = entry;
    }
}
