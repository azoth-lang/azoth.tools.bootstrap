namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

public sealed class InheritedAttributeEquationSyntax : EquationSyntax
{
    public SelectorSyntax Selector { get; }
    public string Name { get; }
    public bool IsMethod { get; }
    public string? Expression { get; }

    public InheritedAttributeEquationSyntax(
        SymbolSyntax node,
        SelectorSyntax selector,
        string name,
        bool isMethod,
        string? expression)
        : base(node)
    {
        Selector = selector;
        Name = name;
        IsMethod = isMethod;
        Expression = expression;
    }
}
