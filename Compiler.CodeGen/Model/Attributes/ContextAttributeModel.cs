using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeKins;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

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

    public abstract ContextAttributeKinModel AttributeSupertype { get; }

    public abstract EvaluationStrategy Strategy { get; }

    public sealed override bool IsSyncLockRequired
        => Strategy == EvaluationStrategy.Lazy && Type.IsValueType;

    protected ContextAttributeModel(AspectModel aspect, InternalSymbol nodeSymbol, string name, bool isMethod, TypeModel type)
        : base(aspect, nodeSymbol, name, isMethod, type) { }

    protected ContextAttributeModel(AspectModel aspect, TreeNodeModel node, string name, bool isMethod, TypeModel type)
        : base(aspect, node, name, isMethod, type) { }

    protected T ComputeAttributeSupertype<T>()
        where T : ContextAttributeKinModel
        => Aspect.Tree.AllAttributeFamilies.OfType<T>()
                 .Single(s => s.Name == Name);

    public sealed override string ToString()
    {
        var strategy = Strategy.ToSourceString();
        var parameters = IsMethod ? "()" : "";
        return $"{Prefix} {strategy} {Node.Defines}.{Name}{parameters}: {Type};";
    }
}
