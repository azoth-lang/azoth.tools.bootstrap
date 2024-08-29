using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FunctionDefinitionNode : PackageMemberDefinitionNode, IFunctionDefinitionNode
{
    public override IFunctionDefinitionSyntax Syntax { get; }
    public override IdentifierName Name => Syntax.Name;
    public override INamespaceDeclarationNode ContainingDeclaration => (INamespaceDeclarationNode)base.ContainingDeclaration;
    public override NamespaceSymbol ContainingSymbol => (NamespaceSymbol)base.ContainingSymbol;
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public override LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.FunctionDefinition_LexicalScope);
    private FunctionSymbol? symbol;
    private bool symbolCached;
    public override FunctionSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol, SymbolsAspect.FunctionDefinition_Symbol);
    public IFixedList<INamedParameterNode> Parameters { get; }
    IFixedList<IConstructorOrInitializerParameterNode> IInvocableDefinitionNode.Parameters => Parameters;
    public ITypeNode? Return { get; }
    public IBodyNode Body { get; }
    private FunctionType? type;
    private bool typeCached;
    public FunctionType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, TypeMemberDeclarationsAspect.FunctionDefinition_Type);
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.InvocableDefinition_ValueIdScope);
    public IEntryNode Entry { get; }
    public IExitNode Exit { get; }
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.ConcreteInvocableDefinition_VariableBindingsMap);

    public FunctionDefinitionNode(
        IFunctionDefinitionSyntax syntax,
        IEnumerable<IAttributeNode> attributes,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return,
        IBodyNode body)
        : base(attributes)
    {
        Syntax = syntax;
        Parameters = ChildList.Attach(this, parameters);
        Return = Child.Attach(this, @return);
        Body = Child.Attach(this, body);
        Entry = Child.Attach(this, new EntryNode());
        Exit = Child.Attach(this, new ExitNode());
    }

    internal override bool Inherited_IsAttributeType(IChildNode child, IChildNode descendant)
        => false;

    internal override LexicalScope Inherited_ContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == Body)
            return LexicalScope;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    public IFlowState FlowStateBefore()
        => TypeMemberDeclarationsAspect.ConcreteInvocable_FlowStateBefore(this);

    internal override IPreviousValueId Previous_PreviousValueId(IChildNode before, IInheritanceContext ctx)
        => ValueIdsAspect.InvocableDefinition_PreviousValueId(this);

    internal override IFlowState Inherited_FlowStateBefore(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (child == Body)
            return Parameters.LastOrDefault()?.FlowStateAfter ?? FlowStateBefore();
        if (Parameters.IndexOf(child) is int index)
        {
            if (index == 0)
                return FlowStateBefore();
            return Parameters[index - 1].FlowStateAfter;
        }
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    internal override IMaybeExpressionAntetype? InheritedExpectedAntetype(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == Body) return Type.Return.Type.ToAntetype();
        return base.InheritedExpectedAntetype(child, descendant, ctx);
    }

    internal override DataType? InheritedExpectedType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == Body) return Type.Return.Type;
        return base.InheritedExpectedType(child, descendant, ctx);
    }

    internal override DataType? Inherited_ExpectedReturnType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == Body) return Type.Return.Type;
        return base.Inherited_ExpectedReturnType(child, descendant, ctx);
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
        => VariableBindingsMap;

    internal override ControlFlowSet Inherited_ControlFlowFollowing(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (descendant == Entry)
            return ControlFlowAspect.ConcreteInvocableDefinition_Entry_ControlFlowFollowing(this);
        if (child == Body) return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IEntryNode Inherited_ControlFlowEntry(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => Entry;

    internal override IExitNode Inherited_ControlFlowExit(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
        => Exit;
}
