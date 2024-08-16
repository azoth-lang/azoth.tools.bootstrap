using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

/// <summary>
/// The semantic model for an attribute declared in an aspect separate from a node
/// </summary>
[Closed(typeof(SynthesizedAttributeModel))]
public abstract class AspectAttributeModel : AttributeModel
{
    public static AspectAttributeModel Create(AspectModel aspect, AspectAttributeSyntax syntax)
        => syntax switch
        {
            SynthesizedAttributeSyntax syn => new SynthesizedAttributeModel(aspect, syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    public AspectModel Aspect { get; }
    public abstract override AttributeSyntax Syntax { get; }
    public InternalSymbol NodeSymbol { get; }
    public override string Name => Syntax.Name;

    protected AspectAttributeModel(AspectModel aspect, SymbolSyntax node)
        : base()
    {
        Aspect = aspect;
        NodeSymbol = Symbol.CreateInternalFromSyntax(aspect.Tree, node);
    }
}
