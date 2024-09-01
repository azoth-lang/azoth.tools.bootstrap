using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

/// <summary>
/// The semantic model for an attribute declared in an aspect separate from a node
/// </summary>
[Closed(
    typeof(LocalAttributeModel),
    typeof(ContextAttributeModel),
    typeof(IntertypeMethodAttributeModel),
    typeof(AggregateAttributeModel))]
public abstract class AspectAttributeModel : AttributeModel
{
    public static AspectAttributeModel Create(AspectModel aspect, AspectAttributeSyntax syntax)
        => syntax switch
        {
            SynthesizedAttributeSyntax syn => new SynthesizedAttributeModel(aspect, syn),
            CircularAttributeSyntax syn => new CircularAttributeModel(aspect, syn),
            InheritedAttributeSyntax syn => new InheritedAttributeModel(aspect, syn),
            PreviousAttributeSyntax syn => new PreviousAttributeModel(aspect, syn),
            IntertypeMethodAttributeSyntax syn => new IntertypeMethodAttributeModel(aspect, syn),
            AggregateAttributeSyntax syn => new AggregateAttributeModel(aspect, syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    public AspectModel Aspect { get; }
    public abstract override AspectAttributeSyntax? Syntax { get; }
    public override bool IsChild => Syntax?.IsChild ?? false;
    public InternalSymbol NodeSymbol { get; }
    public override TreeNodeModel Node => node.Value;
    private readonly Lazy<TreeNodeModel> node;
    public sealed override string Name { get; }
    public override bool IsMethod { get; }
    public override bool IsTemp => IsChild && (Type.ReferencedNode()?.IsTemp ?? false);
    public override bool MayHaveRewrites => IsChild && (Type.ReferencedNode()?.MayHaveRewrite ?? false);

    protected AspectAttributeModel(AspectModel aspect, InternalSymbol nodeSymbol, string name, bool isMethod)
    {
        Aspect = aspect;
        NodeSymbol = nodeSymbol;
        Name = name;
        IsMethod = isMethod;

        node = new(() => aspect.Tree.NodeFor(NodeSymbol)
                         ?? throw new($"Attribute '{Syntax}' refers to node '{NodeSymbol}' that does not exist."));
    }

    protected AspectAttributeModel(AspectModel aspect, TreeNodeModel node, string name, bool isMethod)
    {
        Aspect = aspect;
        Name = name;
        IsMethod = isMethod;
        NodeSymbol = node.Defines;

        this.node = new(node);
    }

    protected T ComputeAttributeFamily<T>()
        where T : AttributeFamilyModel
        => Aspect.Tree.AllAttributeFamilies.OfType<T>()
                 .Single(s => s.Name == Name);
}
