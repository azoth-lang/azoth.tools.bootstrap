namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed record PropertySyntax(string Name, TypeSyntax Type)
{
    public override string ToString() => $"{Name}:{Type}";
}
