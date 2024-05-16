using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class StandardMethodDefinitionNode : MethodDefinitionNode, IStandardMethodDefinitionNode
{
    public override IStandardMethodDefinitionSyntax Syntax { get; }
    public IBodyNode Body { get; }
    public override LexicalScope LexicalScope => throw new NotImplementedException();

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

    internal override Pseudotype InheritedSelfType(IChildNode caller, IChildNode child)
        => TypeExpressionsAspect.ConcreteMethodDeclaration_InheritedSelfType(this);
}
