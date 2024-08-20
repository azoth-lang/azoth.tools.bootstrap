using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeSupertypes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

public sealed class PreviousAttributeModel : AspectAttributeModel
{
    public InheritedAttributeSupertypeModel AttributeSupertype => attributeSupertype.Value;
    private readonly Lazy<InheritedAttributeSupertypeModel> attributeSupertype;

    public override PreviousAttributeSyntax? Syntax { get; }

    public EvaluationStrategy Strategy { get; }

    public PreviousAttributeModel(AspectModel aspect, PreviousAttributeSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name, syntax.IsMethod,
            TypeModel.CreateFromSyntax(aspect.Tree, syntax.Type))
    {
        if (syntax.Strategy == EvaluationStrategy.Eager)
            throw new FormatException($"{syntax.Node}.{syntax.Name} inherited attributes cannot be eager.");
        if (syntax.Strategy is not null && syntax.IsMethod)
            throw new FormatException($"{syntax.Node}.{syntax.Name} cannot specify evaluation strategy for method.");

        Syntax = syntax;
        Strategy = syntax.IsMethod ? EvaluationStrategy.Computed : syntax.Strategy ?? EvaluationStrategy.Lazy;
        attributeSupertype = new(ComputeAttributeSupertype);
    }

    private PreviousAttributeModel(
        AspectModel aspect,
        EvaluationStrategy strategy,
        TreeNodeModel node,
        string name,
        bool isMethod,
        TypeModel type)
        : base(aspect, node, name, isMethod, type)
    {
        Strategy = strategy;
        attributeSupertype = new(ComputeAttributeSupertype);
    }

    private InheritedAttributeSupertypeModel ComputeAttributeSupertype()
        => Aspect.Tree.AllAttributeSupertypes.OfType<InheritedAttributeSupertypeModel>()
                 .Single(s => s.Name == Name);

    public override string ToString()
    {
        var strategy = Strategy.ToSourceString();
        var parameters = IsMethod ? "()" : "";
        return $"⮡ {strategy} {Node.Defines}.{Name}{parameters}: {Type};";
    }
}
