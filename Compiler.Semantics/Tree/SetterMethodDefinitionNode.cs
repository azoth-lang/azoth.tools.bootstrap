using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class SetterMethodDefinitionNode : MethodDefinitionNode, ISetterMethodDefinitionNode
{
    public override ISetterMethodDefinitionSyntax Syntax { get; }
    public IBodyNode Body { get; }
    public override LexicalScope LexicalScope => throw new NotImplementedException();
    IStructMemberSymbolNode IStructMemberDefinitionNode.SymbolNode => SymbolNode;

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
}
