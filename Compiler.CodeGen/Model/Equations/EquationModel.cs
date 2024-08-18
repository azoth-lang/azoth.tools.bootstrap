using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

[Closed(typeof(SynthesizedAttributeEquationModel), typeof(InheritedAttributeEquationModel))]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract class EquationModel
{
    public static EquationModel Create(AspectModel aspect, EquationSyntax syntax)
        => syntax switch
        {
            SynthesizedAttributeEquationSyntax syn => new SynthesizedAttributeEquationModel(aspect, syn),
            InheritedAttributeEquationSyntax syn => new InheritedAttributeEquationModel(aspect, syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    public AspectModel Aspect { get; }
    public abstract EquationSyntax? Syntax { get; }
    public InternalSymbol NodeSymbol { get; }
    public TreeNodeModel Node => node.Value;
    private readonly Lazy<TreeNodeModel> node;
    public string Name { get; }

    protected EquationModel(AspectModel aspect, InternalSymbol nodeSymbol, string name)
    {
        Aspect = aspect;
        NodeSymbol = nodeSymbol;
        Name = name;
        node = new(() => Aspect.Tree.NodeFor(NodeSymbol)
                         ?? throw new($"Attribute '{this}' refers to node '{NodeSymbol}' that does not exist."));
    }

    protected T GetAttribute<T>()
        where T : AttributeModel
        => Aspect.Tree.AttributeFor<T>(NodeSymbol, Name)
           ?? throw new($"{NodeSymbol}.{Name} doesn't have a corresponding attribute.");

    public abstract override string ToString();
}
