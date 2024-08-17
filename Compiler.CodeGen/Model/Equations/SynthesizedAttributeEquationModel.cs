using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class SynthesizedAttributeEquationModel : EquationModel, IMemberModel
{
    public static IEqualityComparer<SynthesizedAttributeEquationModel> NameComparer { get; }
        = EqualityComparer<SynthesizedAttributeEquationModel>.Create((a1, a2) => a1?.Name == a2?.Name,
            a => HashCode.Combine(a.Name));

    public override SynthesizedAttributeEquationSyntax? Syntax { get; }
    public override SynthesizedAttributeModel Attribute => attribute.Value;
    private readonly Lazy<SynthesizedAttributeModel> attribute;
    public EvaluationStrategy Strategy => strategy.Value;
    private readonly Lazy<EvaluationStrategy> strategy;
    public string Name { get; }
    public string Parameters { get; }
    public TypeModel? TypeOverride { get; }
    public TypeModel Type => TypeOverride ?? Attribute.Type;
    public string? Expression { get; }

    public SynthesizedAttributeEquationModel(AspectModel aspect, SynthesizedAttributeEquationSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node))
    {
        if (syntax.EvaluationStrategy == EvaluationStrategy.Lazy && Expression is not null)
            throw new FormatException($"{syntax.Node}.{syntax.Name} has an expression but is marked as lazy.");

        Syntax = syntax;
        strategy = new(ComputeStrategy); syntax.EvaluationStrategy.WithDefault(syntax.Expression);
        Name = syntax.Name;
        Parameters = syntax.Parameters ?? "";
        TypeOverride = TypeModel.CreateFromSyntax(Aspect.Tree, syntax.TypeOverride);
        Expression = syntax.Expression;
        attribute = new(GetAttribute<SynthesizedAttributeModel>);
    }

    public SynthesizedAttributeEquationModel(TreeNodeModel node, SynthesizedAttributeModel attribute)
        : base(attribute.Aspect, node.Defines)
    {
        strategy = new(attribute.Strategy);
        Name = attribute.Name;
        Parameters = attribute.Parameters;

        this.attribute = new(GetAttribute<SynthesizedAttributeModel>);
    }

    private T GetAttribute<T>()
        where T : AttributeModel
        => Aspect.Tree.AttributeFor<T>(NodeSymbol, Name)
           ?? throw new($"{NodeSymbol}.{Name} doesn't have a corresponding attribute.");

    /// <remarks>
    /// 1. If the strategy is specified in the syntax, use it.
    /// 2. If the expression is specified, use Computed.
    /// 3. Otherwise, use the strategy defined in the attribute.
    /// </remarks>
    private EvaluationStrategy ComputeStrategy()
        => Syntax!.EvaluationStrategy.WithDefault(Expression, Attribute.Strategy);

    public override string ToString() => $"= {NodeSymbol}.{Name}";
}
