namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;

public sealed record PropertyNode(string Name, TypeNode Type)
{
    public override string ToString() => $"{Name}:{Type}";
}
