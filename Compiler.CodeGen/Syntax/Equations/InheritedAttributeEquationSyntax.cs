namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

public sealed class InheritedAttributeEquationSyntax : EquationSyntax
{
    public EvaluationStrategy? Strategy { get; }
    public SelectorSyntax Selector { get; }
    public string Name { get; }
    public bool IsMethod { get; }
    public TypeSyntax? TypeOverride { get; }

    public InheritedAttributeEquationSyntax(
        EvaluationStrategy? strategy,
        SymbolSyntax node,
        SelectorSyntax selector,
        string name,
        bool isMethod,
        TypeSyntax? typeOverride)
        : base(node)
    {
        Strategy = strategy;
        Selector = selector;
        Name = name;
        IsMethod = isMethod;
        TypeOverride = typeOverride;
    }
}
