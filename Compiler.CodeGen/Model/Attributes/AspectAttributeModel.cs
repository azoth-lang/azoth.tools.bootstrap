using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

/// <summary>
/// The semantic model for an attribute declared in an aspect separate from a node
/// </summary>
[Closed(typeof(SynthesizedAttributeModel))]
public abstract class AspectAttributeModel : AttributeModel
{
    public AspectModel Aspect { get; }

    protected AspectAttributeModel(AspectModel aspect, SymbolSyntax node)
        : base(aspect.Tree, node)
    {
        Aspect = aspect;
    }

}
