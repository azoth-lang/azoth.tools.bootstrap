using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
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

    public override ConditionalLexicalScope FlowLexicalScope() => Pattern.FlowLexicalScope();

    internal override IMaybeAntetype Inherited_ContextBindingAntetype(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (descendant == Pattern)
            return NameBindingAntetypesAspect.BindingContextPattern_Pattern_ContextBindingAntetype(this);
        return base.Inherited_ContextBindingAntetype(child, descendant, ctx);
    }

    internal override DataType Inherited_ContextBindingType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (descendant == Pattern)
            return NameBindingTypesAspect.BindingContextPattern_Pattern_ContextBindingType(this);
        return base.Inherited_ContextBindingType(child, descendant, ctx);
    }

    protected override ControlFlowSet ComputeControlFlow()
        => ControlFlowAspect.BindingContextPattern_ControlFlowNext(this);
}
