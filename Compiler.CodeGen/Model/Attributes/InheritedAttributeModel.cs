using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

public sealed class InheritedAttributeModel : AspectAttributeModel
{
    public override InheritedAttributeSyntax Syntax { get; }
    public EvaluationStrategy Strategy { get; }
    public bool IsMethod => Syntax.IsMethod;

    public InheritedAttributeModel(AspectModel aspect, InheritedAttributeSyntax syntax)
        : base(aspect, syntax.Node, syntax.Type)
    {
        if (syntax.Strategy == EvaluationStrategy.Eager)
            throw new FormatException($"{syntax.Node}.{syntax.Name} inherited attributes cannot be eager.");
        if (syntax.Strategy is not null && syntax.IsMethod)
            throw new FormatException($"{syntax.Node}.{syntax.Name} cannot specify evaluation strategy for method.");

        Syntax = syntax;
        Strategy = syntax.IsMethod ? EvaluationStrategy.Computed
            : syntax.Strategy ?? EvaluationStrategy.Lazy;
    }
}
