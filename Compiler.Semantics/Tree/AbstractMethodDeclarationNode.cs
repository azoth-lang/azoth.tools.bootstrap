using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AbstractMethodDeclarationNode : MethodDeclarationNode, IAbstractMethodDeclarationNode
{
    public override IAbstractMethodDeclarationSyntax Syntax { get; }
    public override LexicalScope LexicalScope => throw new NotImplementedException();
    public override IDeclarationSymbolNode SymbolNode => throw new NotImplementedException();

    public AbstractMethodDeclarationNode(
        IAbstractMethodDeclarationSyntax syntax,
        IMethodSelfParameterNode selfParameter,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return)
        : base(selfParameter, parameters, @return)
    {
        Syntax = syntax;
    }
}
