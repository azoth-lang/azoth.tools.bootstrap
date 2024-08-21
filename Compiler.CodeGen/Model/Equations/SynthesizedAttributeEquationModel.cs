using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class SynthesizedAttributeEquationModel : EquationModel
{
    public static IEqualityComparer<SynthesizedAttributeEquationModel> NameComparer { get; }
        = EqualityComparer<SynthesizedAttributeEquationModel>.Create((a1, a2) => a1?.Name == a2?.Name,
            a => HashCode.Combine(a.Name));

    public override SynthesizedAttributeEquationSyntax? Syntax { get; }
    public SynthesizedAttributeModel Attribute => attribute.Value;
    private readonly Lazy<SynthesizedAttributeModel> attribute;
    public EvaluationStrategy Strategy => strategy.Value;
    private readonly Lazy<EvaluationStrategy> strategy;
    public TypeModel? TypeOverride { get; }
    public TypeModel Type => TypeOverride ?? Attribute.Type;
    public override bool IsSyncLockRequired
        => Strategy == EvaluationStrategy.Lazy && Type.IsValueType;

    public SynthesizedAttributeEquationModel(AspectModel aspect, SynthesizedAttributeEquationSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name, syntax.IsMethod, syntax.Expression)
    {
        if (syntax.Strategy == EvaluationStrategy.Lazy && Expression is not null)
            throw new FormatException($"{syntax.Node}.{syntax.Name} has an expression but is marked as lazy.");
        if (syntax.Strategy is not null && syntax.IsMethod)
            throw new FormatException($"{syntax.Node}.{syntax.Name} cannot specify evaluation strategy for method.");

        Syntax = syntax;
        strategy = new(ComputeStrategy);
        TypeOverride = TypeModel.CreateFromSyntax(Aspect.Tree, syntax.TypeOverride);
        attribute = new(GetAttribute<SynthesizedAttributeModel>);
    }

    public SynthesizedAttributeEquationModel(TreeNodeModel node, SynthesizedAttributeModel attribute)
        : base(attribute.Aspect, node.Defines, attribute.Name, attribute.IsMethod, attribute.DefaultExpression)
    {
        strategy = new(attribute.Strategy);

        this.attribute = new(GetAttribute<SynthesizedAttributeModel>);
    }

    /// <remarks>
    /// 1. If the strategy is specified in the syntax, use it.
    /// 2. If the expression is specified, use Computed.
    /// 3. Otherwise, use the strategy defined in the attribute.
    /// </remarks>
    private EvaluationStrategy ComputeStrategy()
    {
        if (IsMethod)
            return EvaluationStrategy.Computed;
        return Syntax!.Strategy.WithDefault(Expression, Attribute.Strategy);
    }

    public override string ToString()
    {
        var parameters = IsMethod ? "()" : "";
        return $"= {NodeSymbol}.{Name}{parameters}";
    }
}
