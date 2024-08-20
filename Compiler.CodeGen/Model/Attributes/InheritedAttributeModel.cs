using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

public sealed class InheritedAttributeModel : AspectAttributeModel
{
    public static InheritedAttributeModel? TryMerge(TreeNodeModel node, IEnumerable<InheritedAttributeModel> attributes)
    {
        var (first, rest) = attributes;
        if (first is null)
            return null;
        var aspect = first.Aspect;
        var strategy = first.Strategy;
        var name = first.Name;
        var type = first.Type;
        var isMethod = first.IsMethod;
        foreach (var attribute in rest)
            if (aspect != attribute.Aspect
                || strategy != attribute.Strategy
                || name != attribute.Name
                || type != attribute.Type
                || isMethod != attribute.IsMethod)
                return null;
        return new(aspect, strategy, node, name, isMethod, type);
    }

    public InheritedAttributeSupertypeModel AttributeSupertype => attributeSupertype.Value;
    private readonly Lazy<InheritedAttributeSupertypeModel> attributeSupertype;

    public override InheritedAttributeSyntax? Syntax { get; }

    public EvaluationStrategy Strategy { get; }

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
        attributeSupertype = new(ComputeAttributeSupertype);
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
        attributeSupertype = new(ComputeAttributeSupertype);
    }

    private InheritedAttributeSupertypeModel ComputeAttributeSupertype()
        => Aspect.Tree.AllAttributeSupertypes.Single(s => s.Name == Name);

    public override string ToString()
    {
        var strategy = Strategy.ToSourceString();
        var parameters = IsMethod ? "()" : "";
        return $"↓ {strategy} {Node.Defines}.{Name}{parameters}: {Type};";
    }
}
