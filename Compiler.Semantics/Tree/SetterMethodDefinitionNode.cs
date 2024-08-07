using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class SetterMethodDefinitionNode : MethodDefinitionNode, ISetterMethodDefinitionNode
{
    public override ISetterMethodDefinitionSyntax Syntax { get; }
    public override IBodyNode Body { get; }
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public override LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.ConcreteMethod_LexicalScope, ReferenceEqualityComparer.Instance);
    private FixedDictionary<ILocalBindingNode, int>? localBindingsMap;
    private bool localBindingsMapCached;
    public FixedDictionary<ILocalBindingNode, int> LocalBindingsMap
        => GrammarAttribute.IsCached(in localBindingsMapCached) ? localBindingsMap!
            : this.Synthetic(ref localBindingsMapCached, ref localBindingsMap,
                AssignmentAspect.ConcreteInvocableDefinition_LocalBindingsMap);

    public SetterMethodDefinitionNode(
        ISetterMethodDefinitionSyntax syntax,
        IMethodSelfParameterNode selfParameter,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return,
        IBodyNode body)
        : base(selfParameter, parameters, @return)
    {
        Syntax = syntax;
        Body = Child.Attach(this, body);
    }

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == Body) return LexicalScope;
        return base.InheritedContainingLexicalScope(child, descendant, ctx);
    }

    internal override FixedDictionary<ILocalBindingNode, int> InheritedLocalBindingsMap(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => LocalBindingsMap;

    internal override FixedDictionary<IControlFlowNode, ControlFlowKind> InheritedControlFlowFollowing(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (descendant == Entry)
            return ControlFlowAspect.ConcreteInvocableDefinition_InheritedControlFlowFollowing_Entry(this);
        if (child == Body) return ControlFlowSet.CreateNormal(Exit);
        return base.InheritedControlFlowFollowing(child, descendant, ctx);
    }
}
