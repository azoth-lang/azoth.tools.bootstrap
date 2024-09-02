using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

public sealed class SynthesizedAttributeModel : LocalAttributeModel
{
    public static SynthesizedAttributeModel? TryMerge(TreeNodeModel node, IEnumerable<SynthesizedAttributeModel> attributes)
    {
        var (first, rest) = attributes;
        if (first is null)
            return null;
        var aspect = first.Aspect;
        var strategy = first.Strategy;
        var name = first.Name;
        var type = first.Type;
        var isMethod = first.IsMethod;
        var defaultExpression = first.DefaultExpression;
        foreach (var attribute in rest)
        {
            defaultExpression ??= attribute.DefaultExpression;
            if (aspect != attribute.Aspect
               || strategy != attribute.Strategy
               || name != attribute.Name
               || type != attribute.Type
               || isMethod != attribute.IsMethod
               || defaultExpression != attribute.DefaultExpression)
                return null;
        }
        return new(aspect, strategy, node, name, type, isMethod, defaultExpression);
    }

    public override SynthesizedAttributeSyntax? Syntax { get; }

    public override EvaluationStrategy Strategy { get; }
    public override TypeModel Type { get; }

    public SynthesizedAttributeModel(AspectModel aspect, SynthesizedAttributeSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name,
            syntax.IsMethod, syntax.DefaultExpression)
    {
        if (syntax.Strategy is not null && syntax.IsMethod)
            throw new FormatException($"{syntax.Node}.{syntax.Name} cannot specify evaluation strategy for method.");

        Syntax = syntax;
        Strategy = syntax.IsMethod ? EvaluationStrategy.Computed
            : syntax.Strategy.WithDefault(syntax.DefaultExpression);
        Type = TypeModel.CreateFromSyntax(aspect.Tree, syntax.Type);
    }

    private SynthesizedAttributeModel(
        AspectModel aspect,
        EvaluationStrategy strategy,
        TreeNodeModel node,
        string name,
        TypeModel type,
        bool isMethod,
        string? defaultExpression)
        : base(aspect, node, name, isMethod, defaultExpression)
    {
        Strategy = strategy;
        Type = type;
    }

    public override string ToString()
    {
        var strategy = Strategy.ToSourceString();
        var parameters = IsMethod ? "()" : "";
        var defaultExpression = DefaultExpression is not null ? $" => {DefaultExpression}" : "";
        return $"↑ {strategy} {NodeSymbol}.{Name}{parameters}: {Type}{defaultExpression};";
    }
}
