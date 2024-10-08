using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

/// <summary>
/// An attribute whose value is computed from the node it is attached to.
/// </summary>
/// <remarks>From the name, it seems this would include <see cref="IntertypeMethodAttributeModel"/>
/// however, it really exists as the target for equations that are distinct from that.</remarks>
[Closed(
    typeof(SynthesizedAttributeModel),
    typeof(CircularAttributeModel))]
public abstract class LocalAttributeModel : AspectAttributeModel
{
    public abstract EvaluationStrategy Strategy { get; }
    public string? DefaultExpression { get; }

    protected LocalAttributeModel(AspectModel aspect, InternalSymbol nodeSymbol, string name, bool isMethod, string? defaultExpression)
        : base(aspect, nodeSymbol, name, isMethod)
    {
        DefaultExpression = defaultExpression;
    }

    protected LocalAttributeModel(AspectModel aspect, TreeNodeModel node, string name, bool isMethod, string? defaultExpression)
        : base(aspect, node, name, isMethod)
    {
        DefaultExpression = defaultExpression;
    }
}
