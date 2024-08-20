using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class SetterMethodDefinitionNode : MethodDefinitionNode, ISetterMethodDefinitionNode
{
    public override ISetterMethodDefinitionSyntax Syntax { get; }
    public override MethodKind Kind => MethodKind.Setter;
    public override IBodyNode Body { get; }
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public override LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.MethodDefinition_LexicalScope, ReferenceEqualityComparer.Instance);
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.ConcreteInvocableDefinition_VariableBindingsMap);

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

    internal override FixedDictionary<IVariableBindingNode, int> InheritedVariableBindingsMap(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
        => VariableBindingsMap;

    internal override ControlFlowSet InheritedControlFlowFollowing(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (descendant == Entry)
            return ControlFlowAspect.ConcreteInvocableDefinition_Entry_ControlFlowFollowing(this);
        if (child == Body) return ControlFlowSet.CreateNormal(Exit);
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
