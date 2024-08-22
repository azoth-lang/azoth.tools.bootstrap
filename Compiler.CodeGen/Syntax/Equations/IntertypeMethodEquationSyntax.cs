namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

public sealed class IntertypeMethodEquationSyntax : EquationSyntax
{
    public string Parameters { get; }
    public TypeSyntax? TypeOverride { get; }

    public IntertypeMethodEquationSyntax(
        SymbolSyntax node,
        string name,
        string parameters,
        TypeSyntax? typeOverride,
        string expression)
        : base(node, name, true, expression)
    {
        Parameters = parameters;
        TypeOverride = typeOverride;
    }

    public override string ToString() => $"= {Node}.{Name}({Parameters})";
}
