using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class SynthesizedAttributeEquationModel : SubtreeEquationModel
{
    public static IEqualityComparer<SynthesizedAttributeEquationModel> NameComparer { get; }
        = EqualityComparer<SynthesizedAttributeEquationModel>.Create((a1, a2) => a1?.Name == a2?.Name,
            a => HashCode.Combine(a.Name));

    public override SynthesizedAttributeEquationSyntax? Syntax { get; }
    /// <summary>
    /// The <see cref="SynthesizedAttributeModel"/> that this equation provides a value for.
    /// </summary>
    public override SynthesizedAttributeModel Attribute => attribute.Value;
    private readonly Lazy<SynthesizedAttributeModel> attribute;
    public override EvaluationStrategy Strategy => strategy.Value;
    private readonly Lazy<EvaluationStrategy> strategy;
    public TypeModel? TypeOverride { get; }
    public override TypeModel Type => TypeOverride ?? Attribute.Type;
    public override bool IsSyncLockRequired
        => Strategy == EvaluationStrategy.Lazy && Type.IsValueType;
    public override bool RequiresEmitOnNode
        => Strategy == EvaluationStrategy.Computed && !this.IsObjectMember();

    public SynthesizedAttributeEquationModel(AspectModel aspect, SynthesizedAttributeEquationSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name,
            syntax.IsMethod, syntax.Expression)
    {
        if (syntax.Strategy == EvaluationStrategy.Lazy && Expression is not null)
            throw new FormatException($"{syntax.Node}.{syntax.Name} has an expression but is marked as lazy.");
        if (syntax.Strategy is not null && syntax.IsMethod)
            throw new FormatException($"{syntax.Node}.{syntax.Name} cannot specify evaluation strategy for method.");

        Syntax = syntax;
        strategy = new(ComputeStrategy);
        TypeOverride = TypeModel.CreateFromSyntax(Aspect.Tree, syntax.TypeOverride);
        attribute = new(GetAttribute);
    }

    public SynthesizedAttributeEquationModel(TreeNodeModel node, SynthesizedAttributeModel attribute)
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
        if (IsMethod)
            return EvaluationStrategy.Computed;
        return Syntax!.Strategy.WithDefault(Expression, Attribute.Strategy);
    }

    private SynthesizedAttributeModel GetAttribute()
        => Aspect.Tree.AttributeFor<SynthesizedAttributeModel>(NodeSymbol, Name)
           ?? throw new($"{NodeSymbol}.{Name} doesn't have a corresponding attribute.");

    public override string ToString()
    {
        var parameters = IsMethod ? "()" : "";
        return $"= {NodeSymbol}.{Name}{parameters}";
    }
}
