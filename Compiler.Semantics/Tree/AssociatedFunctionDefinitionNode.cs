using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AssociatedFunctionDefinitionNode : TypeMemberDefinitionNode, IAssociatedFunctionDefinitionNode
{
    public override IAssociatedFunctionDefinitionSyntax Syntax { get; }
    public override UserTypeSymbol ContainingSymbol => (UserTypeSymbol)base.ContainingSymbol;
    public IdentifierName Name => Syntax.Name;
    public IFixedList<INamedParameterNode> Parameters { get; }
    // TODO this explicit implementation shouldn't be needed. There must be a bug in the code generator?
    IFixedList<IConstructorOrInitializerParameterNode> IInvocableDefinitionNode.Parameters => Parameters;
    public ITypeNode? Return { get; }
    private ValueAttribute<FunctionSymbol> symbol;
    public FunctionSymbol Symbol
        => symbol.TryGetValue(out var value) ? value
            : symbol.GetValue(this, SymbolAttribute.AssociatedFunctionDeclaration);
    private ValueAttribute<FunctionType> type;
    public FunctionType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, TypeMemberDeclarationsAspect.AssociatedFunctionDeclaration_Type);
    public IBodyNode Body { get; }
    public override LexicalScope LexicalScope => throw new NotImplementedException();
    private ValueAttribute<IAssociatedFunctionSymbolNode> symbolNode;
    public sealed override IAssociatedFunctionSymbolNode SymbolNode
        => symbolNode.TryGetValue(out var value) ? value
            : symbolNode.GetValue(this, SymbolNodeAttributes.AssociatedFunction_SymbolNode);

    public AssociatedFunctionDefinitionNode(
        IAssociatedFunctionDefinitionSyntax syntax,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return,
        IBodyNode body)
    {
        Syntax = syntax;
        Parameters = ChildList.Attach(this, parameters);
        Return = Child.Attach(this, @return);
        Body = Child.Attach(this, body);
    }
}
