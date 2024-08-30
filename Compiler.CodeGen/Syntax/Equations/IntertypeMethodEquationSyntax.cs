namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

public sealed class IntertypeMethodEquationSyntax : EquationSyntax
{
    public string Parameters { get; }

    public IntertypeMethodEquationSyntax(
        SymbolSyntax node,
        string name,
        string parameters,
        string expression)
        : base(node, name, true, expression)
    {
        Parameters = parameters;
    }

    public override string ToString() => $"= {Node}.{Name}({Parameters})";
}
