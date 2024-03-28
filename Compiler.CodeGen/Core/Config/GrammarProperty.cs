namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;

public sealed record GrammarProperty(string Name, GrammarType Type)
{
    public override string ToString() => $"{Name}:{Type}";
}
