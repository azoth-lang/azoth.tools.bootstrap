using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class StandardMethodDeclarationNode : TypeMemberDeclarationNode, IStandardMethodDeclarationNode
{
    public override IStandardMethodDeclarationSyntax Syntax { get; }
    public override LexicalScope LexicalScope => throw new NotImplementedException();
    public override IDeclarationSymbolNode SymbolNode => throw new NotImplementedException();
    public MethodKind Kind => Syntax.Kind;
    public IdentifierName Name => Syntax.Name;
    public IMethodSelfParameterNode SelfParameter { get; }
    public IFixedList<INamedParameterNode> Parameters { get; }
    public ITypeNode? Return { get; }
    public IBodyNode Body => throw new NotImplementedException();

    public StandardMethodDeclarationNode(
        IStandardMethodDeclarationSyntax syntax,
        IMethodSelfParameterNode selfParameter,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return)
    {
        Syntax = syntax;
        SelfParameter = Child.Attach(this, selfParameter);
        Parameters = ChildList.CreateFixed(this, parameters);
        Return = Child.Attach(this, @return);
    }

    internal override Pseudotype? InheritedSelfType(IChildNode caller, IChildNode child)
        => TypeExpressionsAspect.ConcreteMethodDeclaration_InheritedSelfType(this);
}
