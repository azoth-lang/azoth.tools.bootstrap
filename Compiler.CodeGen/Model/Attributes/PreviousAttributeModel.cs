using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

public sealed class PreviousAttributeModel : ContextAttributeModel
{
    public static ContextAttributeModel Create(
        AspectModel aspect,
        EvaluationStrategy strategy,
        TreeNodeModel node,
        string name,
        bool isMethod)
        => new PreviousAttributeModel(aspect, strategy, node, name, isMethod);

    public override char Prefix => '⮡';
    public override string MethodPrefix => "Previous";

    public override PreviousAttributeFamilyModel AttributeFamily => attributeFamily.Value;
    private readonly Lazy<PreviousAttributeFamilyModel> attributeFamily;

    public override PreviousAttributeSyntax? Syntax { get; }

    public override EvaluationStrategy Strategy { get; }

    public override TypeModel Type => AttributeFamily.Type;

    public PreviousAttributeModel(AspectModel aspect, PreviousAttributeSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name, syntax.IsMethod)
    {
        if (syntax.Strategy == EvaluationStrategy.Eager)
            throw new FormatException($"{syntax.Node}.{syntax.Name} previous attributes cannot be eager.");
        if (syntax.Strategy is not null && syntax.IsMethod)
            throw new FormatException($"{syntax.Node}.{syntax.Name} cannot specify evaluation strategy for method.");

        Syntax = syntax;
        Strategy = syntax.IsMethod ? EvaluationStrategy.Computed : syntax.Strategy ?? EvaluationStrategy.Lazy;
        attributeFamily = new(ComputeAttributeFamily<PreviousAttributeFamilyModel>);
    }

    private PreviousAttributeModel(
        AspectModel aspect,
        EvaluationStrategy strategy,
        TreeNodeModel node,
        string name,
        bool isMethod)
        : base(aspect, node, name, isMethod)
    {
        Strategy = strategy;
        attributeFamily = new(ComputeAttributeFamily<PreviousAttributeFamilyModel>);
    }
}
