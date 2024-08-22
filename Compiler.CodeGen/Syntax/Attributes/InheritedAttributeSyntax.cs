using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

public sealed class InheritedAttributeSyntax : AspectAttributeSyntax
{
    public InheritedAttributeSyntax(
        EvaluationStrategy? strategy,
        SymbolSyntax node,
        string name,
        bool isMethod,
        TypeSyntax type)
        : base(strategy, node, name, isMethod, type)
    {
    }

    public override string ToString()
    {
        var strategy = Strategy.ToSourceString();
        if (strategy.Length > 0)
            strategy += " ";
        var parameters = IsMethod ? "()" : "";
        return $"↓ {strategy}{Node}.{Name}{parameters}: {Type};";
    }
}
