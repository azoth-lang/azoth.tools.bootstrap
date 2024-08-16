using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

[Closed(typeof(SynthesizedAttributeModel))]
public abstract class AttributeModel
{
    public static AttributeModel Create(AspectModel aspect, AttributeSyntax syntax)
        => syntax switch
        {
            SynthesizedAttributeSyntax syn => new SynthesizedAttributeModel(aspect, syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    public AspectModel Aspect { get; }
    public abstract AttributeSyntax Syntax { get; }
    public InternalSymbol Node { get; }
    public string Name => Syntax.Name;

    protected AttributeModel(AspectModel aspect, SymbolSyntax node)
    {
        Aspect = aspect;
        Node = Symbol.CreateInternalFromSyntax(Aspect.Tree, node);
    }
}
