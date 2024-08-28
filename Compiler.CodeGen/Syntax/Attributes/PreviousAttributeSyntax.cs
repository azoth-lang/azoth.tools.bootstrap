using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

public sealed class PreviousAttributeSyntax : AspectAttributeSyntax
{
    public override TypeSyntax Type { get; }

    public PreviousAttributeSyntax(
        EvaluationStrategy? strategy,
        SymbolSyntax node,
        string name,
        bool isMethod,
        TypeSyntax type)
        : base(false, strategy, node, name, isMethod)
    {
        Type = type;
    }

    public override string ToString()
    {
        var strategy = Strategy.ToSourceString();
        if (strategy.Length > 0) strategy += " ";
        var parameters = IsMethod ? "()" : "";
        return $"⮡ {strategy}{Node}.{Name}{parameters}: {Type};";
    }
}
