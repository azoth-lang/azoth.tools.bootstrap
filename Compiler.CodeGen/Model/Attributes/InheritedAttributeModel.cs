using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeKins;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

public sealed class InheritedAttributeModel : ContextAttributeModel, IContextAttributeModel
{
    public static ContextAttributeModel Create(
        AspectModel aspect,
        EvaluationStrategy strategy,
        TreeNodeModel node,
        string name,
        bool isMethod,
        TypeModel type)
        => new InheritedAttributeModel(aspect, strategy, node, name, isMethod, type);

    public override char Prefix => '↓';
    public override string MethodPrefix => "Inherited";

    public override InheritedAttributeKinModel AttributeSupertype => attributeSupertype.Value;
    private readonly Lazy<InheritedAttributeKinModel> attributeSupertype;

    public override InheritedAttributeSyntax? Syntax { get; }

    public override EvaluationStrategy Strategy { get; }

    public InheritedAttributeModel(AspectModel aspect, InheritedAttributeSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name,
            syntax.IsMethod, TypeModel.CreateFromSyntax(aspect.Tree, syntax.Type))
    {
        if (syntax.Strategy == EvaluationStrategy.Eager)
            throw new FormatException($"{syntax.Node}.{syntax.Name} inherited attributes cannot be eager.");
        if (syntax.Strategy is not null && syntax.IsMethod)
            throw new FormatException($"{syntax.Node}.{syntax.Name} cannot specify evaluation strategy for method.");

        Syntax = syntax;
        Strategy = syntax.IsMethod ? EvaluationStrategy.Computed
            : syntax.Strategy ?? EvaluationStrategy.Lazy;
        attributeSupertype = new(ComputeAttributeSupertype<InheritedAttributeKinModel>);
    }
    private InheritedAttributeModel(
        AspectModel aspect,
        EvaluationStrategy strategy,
        TreeNodeModel node,
        string name,
        bool isMethod,
        TypeModel type)
        : base(aspect, node, name, isMethod, type)
    {
        Strategy = strategy;
        attributeSupertype = new(ComputeAttributeSupertype<InheritedAttributeKinModel>);
    }
}
