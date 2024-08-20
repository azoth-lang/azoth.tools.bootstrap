namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

public sealed class InheritedAttributeEquationSyntax : EquationSyntax
{
    public SelectorSyntax Selector { get; }

    public InheritedAttributeEquationSyntax(
        SymbolSyntax node,
        SelectorSyntax selector,
        string name,
        bool isMethod,
        string? expression)
        : base(node, name, isMethod, expression)
    {
        Selector = selector;
    }
}
