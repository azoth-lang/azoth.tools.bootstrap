using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class StandardMethodDefinitionNode : MethodDefinitionNode, IStandardMethodDefinitionNode
{
    public override IStandardMethodDefinitionSyntax Syntax { get; }
    public int Arity => Parameters.Count;
    public FunctionType MethodGroupType => Symbol.MethodGroupType;
    public override IBodyNode Body { get; }
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public override LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.ConcreteMethod_LexicalScope, ReferenceEqualityComparer.Instance);

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

    internal override Pseudotype InheritedSelfType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => TypeExpressionsAspect.ConcreteMethodDeclaration_InheritedSelfType(this);

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
    {
        if (child == Body) return LexicalScope;
        return base.InheritedContainingLexicalScope(child, descendant);
    }

    internal override IMaybeExpressionAntetype? InheritedExpectedAntetype(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == Body) return MethodGroupType.Return.Type.ToAntetype();
        return base.InheritedExpectedAntetype(child, descendant, ctx);
    }
}
