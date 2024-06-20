using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class PatternNode : CodeNode, IPatternNode
{
    public abstract override IPatternSyntax Syntax { get; }

    public abstract FlowState FlowStateAfter { get; }

    private protected PatternNode() { }

    public abstract ConditionalLexicalScope GetFlowLexicalScope();

    public IPreviousValueId PreviousValueId()
        => PreviousValueId(GrammarAttribute.CurrentInheritanceContext());

    public new IMaybeAntetype InheritedBindingAntetype() => base.InheritedBindingAntetype();

    public new DataType InheritedBindingType() => base.InheritedBindingType();
}
