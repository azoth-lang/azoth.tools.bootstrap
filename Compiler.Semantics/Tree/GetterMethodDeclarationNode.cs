using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class GetterMethodDeclarationNode : MethodDeclarationNode, IGetterMethodDeclarationNode
{
    public override IGetterMethodDeclarationSyntax Syntax { get; }
    public override ITypeNode Return => base.Return!;
    public IBodyNode Body { get; }
    public override LexicalScope LexicalScope => throw new NotImplementedException();
    IStructMemberSymbolNode IStructMemberDeclarationNode.SymbolNode => SymbolNode;

    public GetterMethodDeclarationNode(
        IGetterMethodDeclarationSyntax syntax,
        IMethodSelfParameterNode selfParameter,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode @return,
        IBodyNode body)
        : base(selfParameter, parameters, @return)
    {
        Syntax = syntax;
        Body = Child.Attach(this, body);
    }
}
