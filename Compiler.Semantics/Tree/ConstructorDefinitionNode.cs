using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
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
    private ValueAttribute<LexicalScope> lexicalScope;
    public override LexicalScope LexicalScope
        => lexicalScope.TryGetValue(out var value) ? value
            : lexicalScope.GetValue(this, LexicalScopingAspect.ConstructorDefinition_LexicalScope);
    public abstract override ConstructorSymbol Symbol { get; }
    public ValueId? ValueId => null;
    public FlowState FlowStateAfter => Body?.FlowStateAfter ?? SelfParameter!.FlowStateAfter;
    private ValueAttribute<ValueIdScope> valueIdScope;
    public ValueIdScope ValueIdScope
        => valueIdScope.TryGetValue(out var value) ? value
            : valueIdScope.GetValue(this, TypeMemberDeclarationsAspect.Invocable_ValueIdScope);

    private protected ConstructorDefinitionNode(
        IEnumerable<IConstructorOrInitializerParameterNode> parameters)
    {
        Parameters = ChildList.Attach(this, parameters);
    }

    public FlowState FlowStateBefore()
        => TypeMemberDeclarationsAspect.ConcreteInvocable_FlowStateBefore(this);

    internal override IPreviousValueId PreviousValueId(IChildNode before)
        => TypeMemberDeclarationsAspect.Invocable_PreviousValueId(this);

    internal override FlowState InheritedFlowStateBefore(IChildNode child, IChildNode descendant)
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

        return base.InheritedFlowStateBefore(child, descendant);
    }
}
