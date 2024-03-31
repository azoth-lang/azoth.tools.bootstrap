namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

public sealed record PropertyNode(string Name, TypeNode Type)
{
    public override string ToString() => $"{Name}:{Type}";
}
