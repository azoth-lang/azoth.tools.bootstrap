namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

public sealed class PreviousAttributeEquationSyntax : EquationSyntax
{
    public PreviousAttributeEquationSyntax(SymbolSyntax node, string name, bool isMethod, string? expression)
        : base(node, name, isMethod, expression) { }

    public override string ToString()
    {
        var parameters = IsMethod ? "()" : "";
        return $"= {Node}.⮡.{Name}{parameters}";
    }
}
