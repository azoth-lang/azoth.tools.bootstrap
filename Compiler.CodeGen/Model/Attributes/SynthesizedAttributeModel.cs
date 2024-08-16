using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

public sealed class SynthesizedAttributeModel : AspectAttributeModel
{
    public override SynthesizedAttributeSyntax Syntax { get; }
    public EvaluationStrategy Strategy { get; }
    public string Parameters => Syntax.Parameters ?? "";
    public string? DefaultExpression => Syntax.DefaultExpression;

    public SynthesizedAttributeModel(AspectModel aspect, SynthesizedAttributeSyntax syntax)
        : base(aspect, syntax.Node, syntax.Type)
    {
        Syntax = syntax;
        Strategy = syntax.Strategy.WithDefault(syntax.DefaultExpression);
    }
}
