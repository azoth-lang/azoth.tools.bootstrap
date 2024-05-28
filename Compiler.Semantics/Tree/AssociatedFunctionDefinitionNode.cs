using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
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
    public override IdentifierName Name => Syntax.Name;
    StandardName INamedDeclarationNode.Name => Name;
    public IFixedList<INamedParameterNode> Parameters { get; }
    // TODO this explicit implementation shouldn't be needed. There must be a bug in the code generator?
    IFixedList<IConstructorOrInitializerParameterNode> IInvocableDefinitionNode.Parameters => Parameters;
    public ITypeNode? Return { get; }
    private ValueAttribute<FunctionSymbol> symbol;
    public override FunctionSymbol Symbol
        => symbol.TryGetValue(out var value) ? value
            : symbol.GetValue(this, SymbolAspect.AssociatedFunctionDeclaration);
    private ValueAttribute<FunctionType> type;
    public FunctionType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, TypeMemberDeclarationsAspect.AssociatedFunctionDeclaration_Type);
    public IBodyNode Body { get; }
    private ValueAttribute<LexicalScope> lexicalScope;
    public override LexicalScope LexicalScope
        => lexicalScope.TryGetValue(out var value) ? value
            : lexicalScope.GetValue(this, LexicalScopingAspect.AssociatedFunctionDefinition_LexicalScope);

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

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
    {
        if (child == Body) return LexicalScope;
        return base.InheritedContainingLexicalScope(child, descendant);
    }

    internal override ISemanticNode? InheritedPredecessor(IChildNode child, IChildNode descendant)
    {
        if (descendant is INamedParameterNode parameter && Parameters.IndexOf(parameter) is int index)
            return index == 0 ? null : Parameters[index - 1];
        if (descendant == Body)
            return Parameters.LastOrDefault();

        return base.InheritedPredecessor(child, descendant);
    }
}
