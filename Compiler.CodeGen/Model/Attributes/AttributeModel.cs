using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

/// <summary>
/// The semantic model for an attribute.
/// </summary>
[Closed(typeof(AspectAttributeModel))]
public abstract class AttributeModel
{
    public static AttributeModel Create(AspectModel aspect, AttributeSyntax syntax)
        => syntax switch
        {
            SynthesizedAttributeSyntax syn => new SynthesizedAttributeModel(aspect, syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    public abstract AttributeSyntax Syntax { get; }
    public InternalSymbol Node { get; }
    public string Name => Syntax.Name;

    protected AttributeModel(TreeModel tree, SymbolSyntax node)
    {
        Node = Symbol.CreateInternalFromSyntax(tree, node);
    }
}
