using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class SetterMethodDeclarationNode : MethodDeclarationNode, ISetterMethodDeclarationNode
{
    public override ISetterMethodDeclarationSyntax Syntax { get; }

    public override LexicalScope LexicalScope => throw new NotImplementedException();
    public override IDeclarationSymbolNode SymbolNode => throw new NotImplementedException();

    public SetterMethodDeclarationNode(
        ISetterMethodDeclarationSyntax syntax,
        IMethodSelfParameterNode selfParameter,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return)
        : base(selfParameter, parameters, @return)
    {
        Syntax = syntax;
    }
}
