using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class LocalAttributeEquationModel : SoleEquationModel
{
    public override LocalAttributeEquationSyntax? Syntax { get; }
    /// <summary>
    /// The attribute that this equation provides a value for.
    /// </summary>
    public override LocalAttributeModel Attribute => attribute.Value;
    private readonly Lazy<LocalAttributeModel> attribute;
    public override EvaluationStrategy Strategy => strategy.Value;
    private readonly Lazy<EvaluationStrategy> strategy;
    public override string? Parameters => IsMethod ? "()" : null;
    public override TypeModel Type => Attribute.Type;
    public override bool IsSyncLockRequired
        => Strategy == EvaluationStrategy.Lazy && Type.IsValueType;
    public override bool RequiresEmitOnNode
        => Strategy == EvaluationStrategy.Computed && !this.IsObjectMember();

    public LocalAttributeEquationModel(AspectModel aspect, LocalAttributeEquationSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name,
            syntax.IsMethod, syntax.Expression)
    {
        if (syntax.Strategy == EvaluationStrategy.Lazy && Expression is not null)
            throw new FormatException($"{syntax.Node}.{syntax.Name} has an expression but is marked as lazy.");
        if (syntax.Strategy is not null && syntax.IsMethod)
            throw new FormatException($"{syntax.Node}.{syntax.Name} cannot specify evaluation strategy for method.");

        Syntax = syntax;
        strategy = new(ComputeStrategy);
        attribute = new(GetAttribute);
    }

    public LocalAttributeEquationModel(TreeNodeModel node, LocalAttributeModel attribute)
        : base(attribute.Aspect, node.Defines, attribute.Name, attribute.IsMethod, attribute.DefaultExpression)
    {
        strategy = new(attribute.Strategy);

        this.attribute = new(GetAttribute);
    }

    /// <remarks>
    /// 1. If the expression is specified, use Computed.
    /// 2. If the strategy is specified in the syntax, use it.
    /// 3. Otherwise, use the strategy defined in the attribute.
    /// </remarks>
    private EvaluationStrategy ComputeStrategy()
    {
        switch (Attribute)
        {
            case CircularAttributeModel _:
                return EvaluationStrategy.Lazy;
            case SynthesizedAttributeModel attr:
                if (IsMethod)
                    return EvaluationStrategy.Computed;
                return Syntax!.Strategy.WithDefault(Expression, attr.Strategy);
            default:
                throw ExhaustiveMatch.Failed(Attribute);
        }
    }

    private LocalAttributeModel GetAttribute()
        => Aspect.Tree.AttributeFor<LocalAttributeModel>(NodeSymbol, Name)
           ?? throw new($"{NodeSymbol}.{Name} doesn't have a corresponding attribute.");

    public override string ToString() => $"= {NodeSymbol}.{Name}{Parameters ?? ""}";
}
