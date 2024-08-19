using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

public sealed class SynthesizedAttributeModel : AspectAttributeModel
{
    public override SynthesizedAttributeSyntax Syntax { get; }

    public EvaluationStrategy Strategy { get; }
    public string? Parameters => Syntax.Parameters;
    public string? DefaultExpression => Syntax.DefaultExpression;

    public SynthesizedAttributeModel(AspectModel aspect, SynthesizedAttributeSyntax syntax)
        : base(aspect, syntax.Node, syntax.Type)
    {
        if (syntax.Strategy is not null && syntax.Parameters is not null)
            throw new FormatException($"{syntax.Node}.{syntax.Name} cannot specify evaluation strategy for method.");

        Syntax = syntax;
        Strategy = syntax.Parameters is not null ? EvaluationStrategy.Computed
            : syntax.Strategy.WithDefault(syntax.DefaultExpression);
    }

    public override string ToString()
    {
        var strategy = Strategy.ToSourceString();
        var expression = DefaultExpression is not null ? $" => {DefaultExpression}" : "";
        return $"↑ {strategy}{Node.Defines}.{Name}{Parameters}: {Type}{expression};";
    }
}
