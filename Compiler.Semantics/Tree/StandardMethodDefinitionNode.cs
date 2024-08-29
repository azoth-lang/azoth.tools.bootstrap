using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class StandardMethodDefinitionNode : MethodDefinitionNode, IStandardMethodDefinitionNode
{
    public override IStandardMethodDefinitionSyntax Syntax { get; }
    public override MethodKind Kind => MethodKind.Standard;
    public int Arity => Parameters.Count;
    public FunctionType MethodGroupType => Symbol.MethodGroupType;
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

    public StandardMethodDefinitionNode(
        IStandardMethodDefinitionSyntax syntax,
        IMethodSelfParameterNode selfParameter,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return,
        IBodyNode body)
        : base(selfParameter, parameters, @return)
    {
        Syntax = syntax;
        Body = Child.Attach(this, body);
    }

    internal override Pseudotype? Inherited_MethodSelfType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => TypeExpressionsAspect.ConcreteMethodDefinition_Children_Broadcast_MethodSelfType(this);

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (child == Body) return LexicalScope;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override IMaybeExpressionAntetype? Inherited_ExpectedAntetype(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (descendant == Body) return MethodGroupType.Return.Type.ToAntetype();
        return base.Inherited_ExpectedAntetype(child, descendant, ctx);
    }

    internal override DataType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (descendant == Body) return MethodGroupType.Return.Type;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(
        SemanticNode child,
        SemanticNode descendant,
        IInheritanceContext ctx)
        => VariableBindingsMap;

    internal override ControlFlowSet Inherited_ControlFlowFollowing(
        SemanticNode child,
        SemanticNode descendant,
        IInheritanceContext ctx)
    {
        if (descendant == Entry)
            return ControlFlowAspect.ConcreteInvocableDefinition_Entry_ControlFlowFollowing(this);
        if (child == Body) return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IEntryNode Inherited_ControlFlowEntry(
        SemanticNode child,
        SemanticNode descendant,
        IInheritanceContext ctx)
        => Entry;

    internal override IExitNode Inherited_ControlFlowExit(
        SemanticNode child,
        SemanticNode descendant,
        IInheritanceContext ctx)
        => Exit;
}
