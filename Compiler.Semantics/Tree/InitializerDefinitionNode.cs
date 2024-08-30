using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class InitializerDefinitionNode : TypeMemberDefinitionNode, IInitializerDefinitionNode
{
    public abstract override IInitializerDefinitionSyntax? Syntax { get; }
    public override IUserTypeDeclarationNode ContainingDeclaration
        => (IUserTypeDeclarationNode)base.ContainingDeclaration;
    public override UserTypeSymbol ContainingSymbol => (UserTypeSymbol)base.ContainingSymbol;
    public override IdentifierName? Name => Syntax?.Name;
    public abstract IInitializerSelfParameterNode? SelfParameter { get; }
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { get; }
    public abstract IBlockBodyNode? Body { get; }
    IBodyNode? IConcreteInvocableDefinitionNode.Body => Body;
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public override LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.InitializerDefinition_LexicalScope,
                ReferenceEqualityComparer.Instance);
    public abstract override InitializerSymbol Symbol { get; }
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

    private protected InitializerDefinitionNode(
        IEnumerable<IConstructorOrInitializerParameterNode> parameters)
    {
        Parameters = ChildList.Attach(this, parameters);
        Entry = Child.Attach(this, new EntryNode());
        Exit = Child.Attach(this, new ExitNode());
    }

    public IFlowState FlowStateBefore()
        => TypeMemberDeclarationsAspect.ConcreteInvocable_FlowStateBefore(this);

    internal override IPreviousValueId Previous_PreviousValueId(SemanticNode before, IInheritanceContext ctx)
        => ValueIdsAspect.InvocableDefinition_PreviousValueId(this);

    internal override IFlowState Inherited_FlowStateBefore(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (child == Body)
            return Parameters.LastOrDefault()?.FlowStateAfter
                   ?? SelfParameter?.FlowStateAfter ?? FlowStateBefore();
        if (Parameters.IndexOf((IChildNode)child) is int index)
        {
            if (index == 0)
                return SelfParameter?.FlowStateAfter ?? FlowStateBefore();
            return Parameters[index - 1].FlowStateAfter;
        }

        if (child == SelfParameter)
            return FlowStateBefore();

        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    internal sealed override DataType? Inherited_ExpectedReturnType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (child == Body) return DataType.Void;
        return base.Inherited_ExpectedReturnType(child, descendant, ctx);
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(
        SemanticNode child,
        SemanticNode descendant,
        IInheritanceContext ctx)
        => VariableBindingsMap;
}
