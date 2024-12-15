using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

/// <summary>
/// Context attributes are those that provide information about the context in which a node is
/// defined.
/// </summary>
[Closed(typeof(InheritedAttributeModel), typeof(PreviousAttributeModel))]
public abstract class ContextAttributeModel : AspectAttributeModel
{
    public static ContextAttributeModel? TryMerge<T>(TreeNodeModel node, IEnumerable<T> attributes)
        where T : ContextAttributeModel, IContextAttributeModel
    {
        var (first, rest) = attributes;
        if (first is null) return null;
        var aspect = first.Aspect;
        var strategy = first.Strategy;
        var name = first.Name;
        var type = first.Type;
        var isMethod = first.IsMethod;
        foreach (var attribute in rest)
            if (aspect != attribute.Aspect || strategy != attribute.Strategy || name != attribute.Name
                || type != attribute.Type || isMethod != attribute.IsMethod)
                return null;
        return T.Create(aspect, strategy, node, name, isMethod, type);
    }

    public abstract char Prefix { get; }

    public abstract string MethodPrefix { get; }

    public abstract ContextAttributeFamilyModel AttributeFamily { get; }

    public abstract EvaluationStrategy Strategy { get; }

    public sealed override bool IsSyncLockRequired
        => Strategy == EvaluationStrategy.Lazy && Type.IsValueType;

    protected ContextAttributeModel(AspectModel aspect, InternalSymbol nodeSymbol, string name, bool isMethod)
        : base(aspect, nodeSymbol, name, isMethod)
    {
    }

    protected ContextAttributeModel(AspectModel aspect, TreeNodeModel node, string name, bool isMethod)
        : base(aspect, node, name, isMethod)
    {
    }

    public sealed override string ToString()
    {
        var strategy = Strategy.ToSourceCodeString();
        var parameters = IsMethod ? "()" : "";
        return $"{Prefix} {strategy} {Node.Defines}.{Name}{parameters}: {Type};";
    }
}
