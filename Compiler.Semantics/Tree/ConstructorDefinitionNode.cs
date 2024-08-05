using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class ConstructorDefinitionNode : TypeMemberDefinitionNode, IConstructorDefinitionNode
{
    public abstract override IConstructorDefinitionSyntax? Syntax { get; }
    public override UserTypeSymbol ContainingSymbol => (UserTypeSymbol)base.ContainingSymbol;
    public override IdentifierName? Name => Syntax?.Name;
    public abstract IConstructorSelfParameterNode? SelfParameter { get; }
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { get; }
    public abstract IBodyNode? Body { get; }
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public override LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.ConstructorDefinition_LexicalScope,
                ReferenceEqualityComparer.Instance);
    public abstract override ConstructorSymbol Symbol { get; }
    private ValueAttribute<ValueIdScope> valueIdScope;
    public ValueIdScope ValueIdScope
        => valueIdScope.TryGetValue(out var value) ? value
            : valueIdScope.GetValue(this, TypeMemberDeclarationsAspect.Invocable_ValueIdScope);
    public IEntryNode Entry { get; }
    public IExitNode Exit { get; }
    private FixedDictionary<ILocalBindingNode, int>? localBindingsMap;
    private bool localBindingsMapCached;
    public FixedDictionary<ILocalBindingNode, int> LocalBindingsMap
        => GrammarAttribute.IsCached(in localBindingsMapCached) ? localBindingsMap!
            : this.Synthetic(ref localBindingsMapCached, ref localBindingsMap,
                AssignmentAspect.ConcreteInvocableDefinition_LocalBindingsMap);

    private protected ConstructorDefinitionNode(
        IEnumerable<IConstructorOrInitializerParameterNode> parameters)
    {
        Parameters = ChildList.Attach(this, parameters);
        Entry = Child.Attach(this, new EntryNode());
        Exit = Child.Attach(this, new ExitNode());
    }

    public IFlowState FlowStateBefore()
        => TypeMemberDeclarationsAspect.ConcreteInvocable_FlowStateBefore(this);

    internal override IPreviousValueId PreviousValueId(IChildNode before, IInheritanceContext ctx)
        => TypeMemberDeclarationsAspect.Invocable_PreviousValueId(this);

    internal override IFlowState InheritedFlowStateBefore(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (child == Body)
            return Parameters.LastOrDefault()?.FlowStateAfter
                   ?? SelfParameter?.FlowStateAfter ?? FlowStateBefore();
        if (Parameters.IndexOf(child) is int index)
        {
            if (index == 0)
                return SelfParameter?.FlowStateAfter ?? FlowStateBefore();
            return Parameters[index - 1].FlowStateAfter;
        }
        if (child == SelfParameter)
            return FlowStateBefore();

        return base.InheritedFlowStateBefore(child, descendant, ctx);
    }

    internal sealed override DataType? InheritedExpectedReturnType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == Body) return DataType.Void;
        return base.InheritedExpectedReturnType(child, descendant, ctx);
    }

    internal override FixedDictionary<ILocalBindingNode, int> InheritedLocalBindingsMap(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => LocalBindingsMap;
}
