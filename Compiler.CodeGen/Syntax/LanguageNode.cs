namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed class LanguageNode
{
    public string? Name { get; }
    public string DefinitionFilePath { get; }
    public GrammarNode Grammar { get; }
    public LanguageNode? Extends { get; }

    public LanguageNode(string? name, string definitionFilePath, GrammarNode grammar, LanguageNode? extends)
    {
        Name = name;
        DefinitionFilePath = definitionFilePath;
        Grammar = grammar;
        Extends = extends;
    }
}
