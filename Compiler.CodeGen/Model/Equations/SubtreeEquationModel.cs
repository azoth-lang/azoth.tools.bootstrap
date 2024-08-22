using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

[Closed(typeof(SynthesizedAttributeEquationModel), typeof(IntertypeMethodEquationModel))]
public abstract class SubtreeEquationModel : EquationModel
{
    public abstract AttributeModel Attribute { get; }
    public abstract EvaluationStrategy Strategy { get; }
    public abstract bool RequiresEmitOnNode { get; }

    protected SubtreeEquationModel(
        AspectModel aspect,
        InternalSymbol nodeSymbol,
        string name,
        bool isMethod,
        string? expression)
        : base(aspect, nodeSymbol, name, isMethod, expression) { }
}
