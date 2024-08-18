using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

public sealed class InheritedAttributeSyntax : AspectAttributeSyntax
{
    public EvaluationStrategy? Strategy { get; }
    public bool IsMethod { get; }
    public TypeSyntax Type { get; }

    public InheritedAttributeSyntax(
        EvaluationStrategy? strategy,
        SymbolSyntax node,
        string name,
        bool isMethod,
        TypeSyntax type)
        : base(node, name)
    {
        Strategy = strategy;
        IsMethod = isMethod;
        Type = type;
    }

    public override string ToString()
    {
        var strategy = Strategy.ToSourceString();
        if (strategy.Length > 0)
            strategy += " ";
        return $"↓ {strategy}{Node}.{Name}: {Type};";
    }
}
