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

internal class ConstructorDefinitionNode : TypeMemberDefinitionNode, IConstructorDefinitionNode
{
    public override IConstructorDefinitionSyntax Syntax { get; }
    public override UserTypeSymbol ContainingSymbol => (UserTypeSymbol)base.ContainingSymbol;
    public IdentifierName? Name => Syntax.Name;
    public IConstructorSelfParameterNode SelfParameter { get; }
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { get; }
    public IBlockBodyNode Body { get; }
    public override LexicalScope LexicalScope => throw new NotImplementedException();
    private ValueAttribute<IConstructorSymbolNode> symbolNode;
    public override IConstructorSymbolNode SymbolNode
        => symbolNode.TryGetValue(out var value) ? value
            : symbolNode.GetValue(this, SymbolNodeAttributes.ConstructorDeclaration_SymbolNode);
    IClassMemberSymbolNode IClassMemberDefinitionNode.SymbolNode => SymbolNode;
    private ValueAttribute<ConstructorSymbol> symbol;
    public ConstructorSymbol Symbol
        => symbol.TryGetValue(out var value) ? value
            : symbol.GetValue(this, SymbolAttribute.ConstructorDeclaration);

    public ConstructorDefinitionNode(
        IConstructorDefinitionSyntax syntax,
        IConstructorSelfParameterNode selfParameter,
        IEnumerable<IConstructorOrInitializerParameterNode> parameters,
        IBlockBodyNode body)
    {
        Syntax = syntax;
        SelfParameter = Child.Attach(this, selfParameter);
        Parameters = ChildList.Attach(this, parameters);
        Body = Child.Attach(this, body);
    }
}
