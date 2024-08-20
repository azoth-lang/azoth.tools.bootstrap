using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

/// <summary>
/// The semantic model for an attribute declared in an aspect separate from a node
/// </summary>
[Closed(typeof(SynthesizedAttributeModel), typeof(InheritedAttributeModel), typeof(PreviousAttributeModel))]
public abstract class AspectAttributeModel : AttributeModel
{
    public static AspectAttributeModel Create(AspectModel aspect, AspectAttributeSyntax syntax)
        => syntax switch
        {
            SynthesizedAttributeSyntax syn => new SynthesizedAttributeModel(aspect, syn),
            InheritedAttributeSyntax syn => new InheritedAttributeModel(aspect, syn),
            PreviousAttributeSyntax syn => new PreviousAttributeModel(aspect, syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    public AspectModel Aspect { get; }
    public abstract override AspectAttributeSyntax? Syntax { get; }
    public InternalSymbol NodeSymbol { get; }
    public override TreeNodeModel Node => node.Value;
    private readonly Lazy<TreeNodeModel> node;
    public sealed override string Name { get; }
    public override bool IsMethod { get; }
    public sealed override TypeModel Type { get; }

    protected AspectAttributeModel(AspectModel aspect, InternalSymbol nodeSymbol, string name, bool isMethod, TypeModel type)
    {
        Aspect = aspect;
        NodeSymbol = nodeSymbol;
        Name = name;
        IsMethod = isMethod;
        Type = type;

        node = new(() => aspect.Tree.NodeFor(NodeSymbol)
                         ?? throw new($"Attribute '{Syntax}' refers to node '{NodeSymbol}' that does not exist."));
    }

    protected AspectAttributeModel(AspectModel aspect, TreeNodeModel node, string name, bool isMethod, TypeModel type)
    {
        Aspect = aspect;
        Name = name;
        IsMethod = isMethod;
        NodeSymbol = node.Defines;
        Type = type;

        this.node = new(node);
    }
}
