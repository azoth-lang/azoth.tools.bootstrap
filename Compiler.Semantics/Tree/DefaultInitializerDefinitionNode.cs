using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal class DefaultInitializerDefinitionNode : InitializerDefinitionNode, IDefaultInitializerDefinitionNode
{
    public override IInitializerDefinitionSyntax? Syntax => null;
    public override IInitializerSelfParameterNode? SelfParameter => null;
    public override IBlockBodyNode? Body => null;
    IBodyNode? IConcreteInvocableDefinitionNode.Body => null;
    private InitializerSymbol? symbol;
    private bool symbolCached;
    public override InitializerSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol, SymbolsAspect.DefaultInitializerDefinition_Symbol);

    public DefaultInitializerDefinitionNode()
        : base(FixedList.Empty<IConstructorOrInitializerParameterNode>()) { }

    internal override ControlFlowSet InheritedControlFlowFollowing(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (descendant == Entry)
            return ControlFlowAspect.ConcreteInvocableDefinition_Entry_ControlFlowFollowing(this);
        return base.InheritedControlFlowFollowing(child, descendant, ctx);
    }

    internal override IEntryNode InheritedControlFlowEntry(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
        => Entry;

    internal override IExitNode InheritedControlFlowExit(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
        => Exit;
}
