using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal class SourceInitializerDefinitionNode : InitializerDefinitionNode, ISourceInitializerDefinitionNode
{
    public override IInitializerDefinitionSyntax Syntax { get; }
    public override IInitializerSelfParameterNode SelfParameter { get; }
    public override IBlockBodyNode Body { get; }
    private InitializerSymbol? symbol;
    private bool symbolCached;
    public override InitializerSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol, SymbolAspect.SourceInitializerDefinition_Symbol);

    public SourceInitializerDefinitionNode(
        IInitializerDefinitionSyntax syntax,
        IInitializerSelfParameterNode selfParameter,
        IEnumerable<IConstructorOrInitializerParameterNode> parameters,
        IBlockBodyNode body)
        : base(parameters)
    {
        Syntax = syntax;
        SelfParameter = Child.Attach(this, selfParameter);
        Body = Child.Attach(this, body);
    }

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == Body) return LexicalScope;
        return base.InheritedContainingLexicalScope(child, descendant, ctx);
    }

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
