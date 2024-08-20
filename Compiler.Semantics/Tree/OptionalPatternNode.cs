using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class OptionalPatternNode : PatternNode, IOptionalPatternNode
{
    public override IOptionalPatternSyntax Syntax { get; }
    public IOptionalOrBindingPatternNode Pattern { get; }
    public override IFlowState FlowStateAfter => Pattern.FlowStateAfter;

    public OptionalPatternNode(IOptionalPatternSyntax syntax, IOptionalOrBindingPatternNode pattern)
    {
        Syntax = syntax;
        Pattern = Child.Attach(this, pattern);
    }

    public override ConditionalLexicalScope FlowLexicalScope() => Pattern.FlowLexicalScope();

    internal override IMaybeAntetype InheritedContextBindingAntetype(IChildNode child, IChildNode descendant)
    {
        if (descendant == Pattern)
            return NameBindingAntetypesAspect.OptionalPattern_Pattern_ContextBindingAntetype(this);
        return base.InheritedContextBindingAntetype(child, descendant);
    }

    internal override DataType InheritedContextBindingType(IChildNode child, IChildNode descendant)
    {
        if (descendant == Pattern)
            return NameBindingTypesAspect.OptionalPattern_Pattern_ContextBindingType(this);
        return base.InheritedContextBindingType(child, descendant);
    }

    protected override void CollectDiagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        ExpressionAntetypesAspect.OptionalPattern_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }

    protected override ControlFlowSet ComputeControlFlow()
        => ControlFlowAspect.OptionalPattern_ControlFlowNext(this);
}
