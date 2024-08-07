using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
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
            : this.Synthetic(ref symbolCached, ref symbol, SymbolAspect.DefaultInitializerDefinition_Symbol);

    public DefaultInitializerDefinitionNode()
        : base(FixedList.Empty<IConstructorOrInitializerParameterNode>()) { }

    internal override ControlFlowSet InheritedControlFlowFollowing(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (descendant == Entry)
            return ControlFlowAspect.ConcreteInvocableDefinition_InheritedControlFlowFollowing_Entry(this);
        if (child == Body) return ControlFlowSet.CreateNormal(Exit);
        return base.InheritedControlFlowFollowing(child, descendant, ctx);
    }

    internal override IControlFlowNode InheritedControlFlowExit(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => Exit;
}
