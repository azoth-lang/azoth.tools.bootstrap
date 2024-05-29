using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FunctionDefinitionNode : PackageMemberDefinitionNode, IFunctionDefinitionNode
{
    public override IFunctionDefinitionSyntax Syntax { get; }
    public override IdentifierName Name => Syntax.Name;
    public override INamespaceDeclarationNode ContainingDeclaration => (INamespaceDeclarationNode)base.ContainingDeclaration;
    public override NamespaceSymbol ContainingSymbol => (NamespaceSymbol)base.ContainingSymbol;
    private ValueAttribute<LexicalScope> lexicalScope;
    public override LexicalScope LexicalScope
        => lexicalScope.TryGetValue(out var value) ? value
            : lexicalScope.GetValue(this, LexicalScopingAspect.FunctionDefinition_LexicalScope);
    private ValueAttribute<FunctionSymbol> symbol;
    public override FunctionSymbol Symbol
        => symbol.TryGetValue(out var value) ? value
            : symbol.GetValue(this, SymbolAspect.FunctionDeclaration);
    public IFixedList<INamedParameterNode> Parameters { get; }
    IFixedList<IConstructorOrInitializerParameterNode> IInvocableDefinitionNode.Parameters => Parameters;
    public ITypeNode? Return { get; }
    public IBodyNode Body { get; }
    private ValueAttribute<FunctionType> type;
    public FunctionType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, TypeMemberDeclarationsAspect.FunctionDeclaration_Type);
    public ValueId? ValueId => null;
    public FlowState FlowStateAfter => Body.FlowStateAfter;
    private ValueAttribute<ValueIdScope> valueIdScope;
    public ValueIdScope ValueIdScope
        => valueIdScope.TryGetValue(out var value) ? value
            : valueIdScope.GetValue(this, TypeMemberDeclarationsAspect.Invocable_ValueIdScope);

    public FunctionDefinitionNode(
        IFunctionDefinitionSyntax syntax,
        IEnumerable<IAttributeNode> attributes,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return,
        IBodyNode body)
        : base(attributes)
    {
        Syntax = syntax;
        Parameters = ChildList.Attach(this, parameters);
        Return = Child.Attach(this, @return);
        Body = Child.Attach(this, body);
    }

    internal override bool InheritedIsAttributeType(IChildNode child, IChildNode descendant)
        => SymbolNodeAspect.FunctionDeclaration_InheritedIsAttributeType(this);

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
    {
        if (child == Body)
            return LexicalScope;
        return base.InheritedContainingLexicalScope(child, descendant);
    }

    internal override IFlowNode InheritedPredecessor(IChildNode child, IChildNode descendant)
    {
        if (child is INamedParameterNode parameter && Parameters.IndexOf(parameter) is int index)
            return index == 0 ? this : Parameters[index - 1];
        if (child == Body)
            return Parameters.LastOrDefault() ?? (IFlowNode)this;

        return base.InheritedPredecessor(child, descendant);
    }

    public IFlowNode Predecessor()
        => TypeMemberDeclarationsAspect.ConcreteInvocable_Predecessor(this);

    public FlowState FlowStateBefore()
        => TypeMemberDeclarationsAspect.ConcreteInvocable_FlowStateBefore(this);

    internal override IPreviousValueId PreviousValueId(IChildNode before)
        => TypeMemberDeclarationsAspect.Invocable_PreviousValueId(this);
}
