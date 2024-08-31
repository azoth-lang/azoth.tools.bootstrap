using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

public sealed class PreviousAttributeSyntax : AspectAttributeSyntax
{
    public override TypeSyntax? Type => null;

    public PreviousAttributeSyntax(
        EvaluationStrategy? strategy,
        SymbolSyntax node,
        string name,
        bool isMethod)
        : base(false, strategy, node, name, isMethod)
    {
    }

    public override string ToString()
    {
        var strategy = Strategy.ToSourceString();
        if (strategy.Length > 0) strategy += " ";
        var parameters = IsMethod ? "()" : "";
        return $"⮡ {strategy}{Node}.{Name}{parameters};";
    }
}
