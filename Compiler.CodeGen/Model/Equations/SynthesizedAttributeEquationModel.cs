using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class SynthesizedAttributeEquationModel : EquationModel
{
    public override SynthesizedAttributeEquationSyntax Syntax { get; }
    public override SynthesizedAttributeModel Attribute => attribute.Value;
    private readonly Lazy<SynthesizedAttributeModel> attribute;
    public EvaluationStrategy EvaluationStrategy { get; }
    public string Name => Syntax.Name;
    public string Parameters => Syntax.Parameters ?? "";
    public TypeModel? TypeOverride { get; }
    public TypeModel Type => TypeOverride ?? Attribute.Type;
    public string? Expression => Syntax.Expression;

    public SynthesizedAttributeEquationModel(AspectModel aspect, SynthesizedAttributeEquationSyntax syntax)
        : base(aspect, syntax.Node)
    {
        Syntax = syntax;
        EvaluationStrategy = ComputeEvaluationStrategy(syntax);
        TypeOverride = TypeModel.CreateFromSyntax(Aspect.Tree, syntax.TypeOverride);
        attribute = new(GetAttribute<SynthesizedAttributeModel>);
    }

    private static EvaluationStrategy ComputeEvaluationStrategy(SynthesizedAttributeEquationSyntax syntax)
        => syntax.EvaluationStrategy
           ?? (syntax.Expression is not null ? EvaluationStrategy.Computed : EvaluationStrategy.Lazy);

    private T GetAttribute<T>()
        where T : AttributeModel
        => Aspect.Tree.AttributeFor<T>(Node, Name)
           ?? throw new($"{Node}.{Name} doesn't have a corresponding attribute.");
}
