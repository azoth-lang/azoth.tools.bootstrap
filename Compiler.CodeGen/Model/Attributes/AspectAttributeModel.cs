using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

/// <summary>
/// The semantic model for an attribute declared in an aspect separate from a node
/// </summary>
[Closed(typeof(SynthesizedAttributeModel), typeof(InheritedAttributeModel))]
public abstract class AspectAttributeModel : AttributeModel
{
    public static AspectAttributeModel Create(AspectModel aspect, AspectAttributeSyntax syntax)
        => syntax switch
        {
            SynthesizedAttributeSyntax syn => new SynthesizedAttributeModel(aspect, syn),
            InheritedAttributeSyntax syn => new InheritedAttributeModel(aspect, syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    public AspectModel Aspect { get; }
    public abstract override AttributeSyntax Syntax { get; }
    public InternalSymbol NodeSymbol { get; }
    public override TreeNodeModel Node => node.Value;
    private readonly Lazy<TreeNodeModel> node;
    public sealed override string Name => Syntax.Name;
    public sealed override TypeModel Type { get; }

    protected AspectAttributeModel(AspectModel aspect, SymbolSyntax node, TypeSyntax type)
    {
        Aspect = aspect;
        NodeSymbol = Symbol.CreateInternalFromSyntax(aspect.Tree, node);
        Type = TypeModel.CreateFromSyntax(aspect.Tree, type);

        this.node = new(() => aspect.Tree.NodeFor(NodeSymbol)
                         ?? throw new($"Attribute '{Syntax}' refers to node '{NodeSymbol}' that does not exist."));
    }
}
