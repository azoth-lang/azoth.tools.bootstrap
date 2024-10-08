using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

[Closed(
    typeof(SoleEquationModel),
    typeof(ContributorEquationModel))]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract class EquationModel : IMemberModel
{
    public static EquationModel Create(AspectModel aspect, EquationSyntax syntax)
        => syntax switch
        {
            LocalAttributeEquationSyntax syn => new LocalAttributeEquationModel(aspect, syn),
            InheritedAttributeEquationSyntax syn => new InheritedAttributeEquationModel(aspect, syn),
            PreviousAttributeEquationSyntax syn => new PreviousAttributeEquationModel(aspect, syn),
            IntertypeMethodEquationSyntax syn => new IntertypeMethodEquationModel(aspect, syn),
            AggregateAttributeEquationSyntax syn => new AggregateAttributeEquationModel(aspect, syn),
            CollectionAttributeEquationSyntax syn => new CollectionAttributeEquationModel(aspect, syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    public AspectModel Aspect { get; }
    public abstract EquationSyntax? Syntax { get; }
    public InternalSymbol NodeSymbol { get; }
    public TreeNodeModel Node => node.Value;
    private readonly Lazy<TreeNodeModel> node;
    public string Name { get; }
    public bool IsMethod { get; }
    public abstract TypeModel Type { get; }
    TypeModel IMemberModel.FinalType => Type;
    bool IMemberModel.IsTemp => false;
    bool IMemberModel.MayHaveRewrites => false;
    bool IMemberModel.IsPlaceholder => false;
    public string? Expression { get; }
    public virtual bool IsSyncLockRequired => false;

    protected EquationModel(
        AspectModel aspect,
        InternalSymbol nodeSymbol,
        string name,
        bool isMethod,
        string? expression)
    {
        Aspect = aspect;
        NodeSymbol = nodeSymbol;
        Name = name;
        IsMethod = isMethod;
        Expression = expression;
        node = new(() => Aspect.Tree.NodeFor(NodeSymbol)
                         ?? throw new($"Attribute '{this}' refers to node '{NodeSymbol}' that does not exist."));
    }

    protected T GetAttribute<T>()
        where T : AttributeModel
        => Aspect.Tree.AttributeFor<T>(NodeSymbol, Name)
           ?? throw new($"{NodeSymbol}.{Name} doesn't have a corresponding attribute.");

    public abstract override string ToString();
}
