using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class DefaultConstructorDefinitionNode : ConstructorDefinitionNode, IDefaultConstructorDefinitionNode
{
    public override IConstructorDefinitionSyntax? Syntax => null;
    public override IConstructorSelfParameterNode? SelfParameter => null;
    public override IBodyNode? Body => null;
    private ConstructorSymbol? symbol;
    private bool symbolCached;
    public override ConstructorSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol, SymbolsAspect.DefaultConstructorDefinition_Symbol);

    public DefaultConstructorDefinitionNode()
        : base(FixedList.Empty<IConstructorOrInitializerParameterNode>())
    { }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(
        SemanticNode child,
        SemanticNode descendant,
        IInheritanceContext ctx)
    {
        if (descendant == Entry)
            return ControlFlowAspect.ConcreteInvocableDefinition_Entry_ControlFlowFollowing(this);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IEntryNode Inherited_ControlFlowEntry(
        SemanticNode child,
        SemanticNode descendant,
        IInheritanceContext ctx)
        => Entry;

    internal override IExitNode Inherited_ControlFlowExit(
        SemanticNode child,
        SemanticNode descendant,
        IInheritanceContext ctx)
        => Exit;
}
