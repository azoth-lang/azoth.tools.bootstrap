using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class BindingContextPatternNode : PatternNode, IBindingContextPatternNode
{
    public override IBindingContextPatternSyntax Syntax { get; }
    public bool IsMutableBinding => Syntax.IsMutableBinding;
    public IPatternNode Pattern { get; }
    public ITypeNode? Type { get; }
    public override IFlowState FlowStateAfter => Pattern.FlowStateAfter;

    public BindingContextPatternNode(
        IBindingContextPatternSyntax syntax,
        IPatternNode pattern,
        ITypeNode? type)
    {
        Syntax = syntax;
        Pattern = Child.Attach(this, pattern);
        Type = Child.Attach(this, type);
    }

    public override ConditionalLexicalScope GetFlowLexicalScope() => Pattern.GetFlowLexicalScope();

    internal override IMaybeAntetype InheritedBindingAntetype(IChildNode child, IChildNode descendant)
    {
        if (descendant == Pattern)
            return NameBindingAntetypesAspect.BindingContextPattern_InheritedBindingAntetype_Pattern(this);
        return base.InheritedBindingAntetype(child, descendant);
    }

    internal override DataType InheritedBindingType(IChildNode child, IChildNode descendant)
    {
        if (descendant == Pattern)
            return NameBindingTypesAspect.BindingContextPattern_InheritedBindingType_Pattern(this);
        return base.InheritedBindingType(child, descendant);
    }
}
