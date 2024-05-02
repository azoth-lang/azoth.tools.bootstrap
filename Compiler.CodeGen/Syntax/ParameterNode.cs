namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed record ParameterNode(string Name, TypeNode Type)
{
    public override string ToString() => $"{Type} {Name}";
}
