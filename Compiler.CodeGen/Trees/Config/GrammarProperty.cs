namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees.Config;

public sealed class GrammarProperty
{
    public string Name { get; }
    public GrammarType Type { get; }

    public GrammarProperty(string name, GrammarType type)
    {
        Name = name;
        Type = type;
    }
}