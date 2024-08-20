using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class MethodDefinitionNode : TypeMemberDefinitionNode, IMethodDefinitionNode
{
    public abstract override IMethodDefinitionSyntax Syntax { get; }
    public override UserTypeSymbol ContainingSymbol => (UserTypeSymbol)base.ContainingSymbol;
    public abstract MethodKind Kind { get; }
    public override IdentifierName Name => Syntax.Name;
    TypeName INamedDeclarationNode.Name => Name;
    public IMethodSelfParameterNode SelfParameter { get; }
    public IFixedList<INamedParameterNode> Parameters { get; }
    public abstract IBodyNode? Body { get; }
    public virtual ITypeNode? Return { get; }
    private MethodSymbol? symbol;
    private bool symbolCached;
    public override MethodSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol, SymbolAspect.MethodDefinition_Symbol);
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.InvocableDefinition_ValueIdScope);
    public IEntryNode Entry { get; }
    public IExitNode Exit { get; }

    private protected MethodDefinitionNode(
        IMethodSelfParameterNode selfParameter,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return)
    {
        SelfParameter = Child.Attach(this, selfParameter);
        Parameters = ChildList.Attach(this, parameters);
        Return = Child.Attach(this, @return);
        Entry = Child.Attach(this, new EntryNode());
        Exit = Child.Attach(this, new ExitNode());
    }

    protected override void CollectDiagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        TypeMemberDeclarationsAspect.MethodDefinition_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }

    public IFlowState FlowStateBefore()
        => TypeMemberDeclarationsAspect.ConcreteInvocable_FlowStateBefore((IConcreteInvocableDefinitionNode)this);

    internal override IPreviousValueId PreviousValueId(IChildNode before, IInheritanceContext ctx)
        => ValueIdsAspect.InvocableDefinition_PreviousValueId(this);

    internal override IFlowState InheritedFlowStateBefore(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (child == Body)
            return Parameters.LastOrDefault()?.FlowStateAfter ?? SelfParameter.FlowStateAfter;
        if (Parameters.IndexOf(child) is int index)
        {
            if (index == 0)
                return SelfParameter.FlowStateAfter;
            return Parameters[index - 1].FlowStateAfter;
        }
        if (child == SelfParameter)
            return FlowStateBefore();

        return base.InheritedFlowStateBefore(child, descendant, ctx);
    }

    internal sealed override DataType? InheritedExpectedReturnType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == Body) return Return?.NamedType ?? DataType.Void;
        return base.InheritedExpectedReturnType(child, descendant, ctx);
    }
}
