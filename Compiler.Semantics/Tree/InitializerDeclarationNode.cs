using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class InitializerDeclarationNode : TypeMemberDeclarationNode, IInitializerDeclarationNode
{
    public override IInitializerDeclarationSyntax Syntax { get; }
    public override UserTypeSymbol ContainingSymbol => (UserTypeSymbol)base.ContainingSymbol;
    public IdentifierName? Name => Syntax.Name;
    public IInitializerSelfParameterNode SelfParameter { get; }
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { get; }
    public IBlockBodyNode Body => throw new NotImplementedException();
    public override LexicalScope LexicalScope => throw new NotImplementedException();
    public override IDeclarationSymbolNode SymbolNode => throw new NotImplementedException();
    private ValueAttribute<InitializerSymbol> symbol;
    public InitializerSymbol Symbol
        => symbol.TryGetValue(out var value) ? value
            : symbol.GetValue(this, SymbolAttribute.InitializerDeclaration);

    public InitializerDeclarationNode(
        IInitializerDeclarationSyntax syntax,
        IInitializerSelfParameterNode selfParameter,
        IEnumerable<IConstructorOrInitializerParameterNode> parameters)
    {
        Syntax = syntax;
        SelfParameter = Child.Attach(this, selfParameter);
        Parameters = ChildList.CreateFixed(this, parameters);
    }
}
