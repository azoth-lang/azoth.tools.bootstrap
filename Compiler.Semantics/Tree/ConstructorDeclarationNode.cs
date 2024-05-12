using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal class ConstructorDeclarationNode : TypeMemberDeclarationNode, IConstructorDeclarationNode
{
    public override IConstructorDeclarationSyntax Syntax { get; }
    public IdentifierName? Name => Syntax.Name;
    public IConstructorSelfParameterNode SelfParameter { get; }
    public IBlockBodyNode Body => throw new NotImplementedException();
    public override LexicalScope LexicalScope => throw new NotImplementedException();
    public override IDeclarationSymbolNode SymbolNode => throw new NotImplementedException();
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { get; }

    public ConstructorDeclarationNode(
        IConstructorDeclarationSyntax syntax,
        IConstructorSelfParameterNode selfParameter,
        IEnumerable<IConstructorOrInitializerParameterNode> parameters)
    {
        Syntax = syntax;
        SelfParameter = Child.Attach(this, selfParameter);
        Parameters = ChildList.CreateFixed(this, parameters);
    }
}
