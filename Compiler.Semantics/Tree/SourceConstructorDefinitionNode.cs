using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class SourceConstructorDefinitionNode : ConstructorDefinitionNode, ISourceConstructorDefinitionNode
{
    public override IConstructorDefinitionSyntax Syntax { get; }
    public override IConstructorSelfParameterNode SelfParameter { get; }
    public override IBlockBodyNode Body { get; }
    private ConstructorSymbol? symbol;
    private bool symbolCached;
    public override ConstructorSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol, SymbolsAspect.SourceConstructorDefinition_Symbol);

    public SourceConstructorDefinitionNode(
        IConstructorDefinitionSyntax syntax,
        IConstructorSelfParameterNode selfParameter,
        IEnumerable<IConstructorOrInitializerParameterNode> parameters,
        IBlockBodyNode body)
        : base(parameters)
    {
        Syntax = syntax;
        SelfParameter = Child.Attach(this, selfParameter);
        Body = Child.Attach(this, body);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (child == Body) return LexicalScope;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (descendant == Entry)
            return ControlFlowAspect.ConcreteInvocableDefinition_Entry_ControlFlowFollowing(this);
        if (child == Body) return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => Entry;

    internal override IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => Exit;
}
