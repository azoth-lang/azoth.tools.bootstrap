using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class InheritedAttributeEquationModel : EquationModel
{
    public override InheritedAttributeEquationSyntax Syntax { get; }
    public override SynthesizedAttributeModel Attribute => attribute.Value;
    private readonly Lazy<SynthesizedAttributeModel> attribute;
    public EvaluationStrategy Strategy => strategy.Value;
    private readonly Lazy<EvaluationStrategy> strategy;
    // Selector
    public bool IsMethod => Syntax.IsMethod;
    public TypeModel? TypeOverride { get; }
    public TypeModel Type => TypeOverride ?? Attribute.Type;

    public InheritedAttributeEquationModel(AspectModel aspect, InheritedAttributeEquationSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name)
    {
        Syntax = syntax;
        TypeOverride = TypeModel.CreateFromSyntax(Aspect.Tree, syntax.TypeOverride);
        strategy = new(ComputeStrategy);
        attribute = new(GetAttribute<SynthesizedAttributeModel>);
    }

    /// <remarks>
    /// 1. If the strategy is specified in the syntax, use it.
    /// 2. If the expression is specified, use Computed.
    /// 3. Otherwise, use the strategy defined in the attribute.
    /// </remarks>
    private EvaluationStrategy ComputeStrategy()
    {
        if (IsMethod) return EvaluationStrategy.Computed;
        return Syntax.Strategy ?? Attribute.Strategy;
    }

    public override string ToString()
    {
        var parameters = IsMethod ? "()" : "";
        return $"= {NodeSymbol}.{Name}{parameters}";
    }
}
