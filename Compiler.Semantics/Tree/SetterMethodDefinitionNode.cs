using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class SetterMethodDefinitionNode : MethodDefinitionNode, ISetterMethodDefinitionNode
{
    public override ISetterMethodDefinitionSyntax Syntax { get; }
    public override IBodyNode Body { get; }
    private ValueAttribute<LexicalScope> lexicalScope;
    public override LexicalScope LexicalScope
        => lexicalScope.TryGetValue(out var value) ? value
            : lexicalScope.GetValue(this, LexicalScopingAspect.ConcreteMethod_LexicalScope);

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

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
    {
        if (child == Body) return LexicalScope;
        return base.InheritedContainingLexicalScope(child, descendant);
    }
}
