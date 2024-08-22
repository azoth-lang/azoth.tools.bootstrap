using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

internal sealed class IntertypeMethodAttributeSyntax : AspectAttributeSyntax
{
    /// <remarks>For the moment, parameters are a single string rather than parsed. This is possible
    /// since the expression is required and therefore refer to the parameters by name.</remarks>
    public string Parameters { get; }
    /// <remarks>For the moment, the expression is required because that avoids the complexity of
    /// passing arguments to computed methods.</remarks>
    public string Expression { get; }

    public IntertypeMethodAttributeSyntax(SymbolSyntax node, string name, string parameters, TypeSyntax type, string expression)
        : base(EvaluationStrategy.Computed, node, name, true, type)
    {
        Parameters = parameters;
        Expression = expression;
    }

    public override string ToString() => $"+ {Node}.{Name}({Parameters}): {Type} => {Expression}";
}
