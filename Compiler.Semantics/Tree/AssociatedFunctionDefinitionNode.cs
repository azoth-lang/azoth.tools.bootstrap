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

internal sealed class AssociatedFunctionDefinitionNode : TypeMemberDefinitionNode, IAssociatedFunctionDefinitionNode
{
    public override IAssociatedFunctionDefinitionSyntax Syntax { get; }
    public override UserTypeSymbol ContainingSymbol => (UserTypeSymbol)base.ContainingSymbol;
    public override IdentifierName Name => Syntax.Name;
    TypeName INamedDeclarationNode.Name => Name;
    public IFixedList<INamedParameterNode> Parameters { get; }
    // TODO this explicit implementation shouldn't be needed. There must be a bug in the code generator?
    IFixedList<IConstructorOrInitializerParameterNode> IInvocableDefinitionNode.Parameters => Parameters;
    public ITypeNode? Return { get; }
    private ValueAttribute<FunctionSymbol> symbol;
    public override FunctionSymbol Symbol
        => symbol.TryGetValue(out var value) ? value
            : symbol.GetValue(this, SymbolAspect.AssociatedFunctionDefinition_Symbol);
    private FunctionType? type;
    private bool typeCached;
    public FunctionType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, TypeMemberDeclarationsAspect.AssociatedFunctionDeclaration_Type);
    public IBodyNode Body { get; }
    private ValueAttribute<LexicalScope> lexicalScope;
    public override LexicalScope LexicalScope
        => lexicalScope.TryGetValue(out var value) ? value
            : lexicalScope.GetValue(this, LexicalScopingAspect.AssociatedFunctionDefinition_LexicalScope);
    private ValueAttribute<ValueIdScope> valueIdScope;
    public ValueIdScope ValueIdScope
        => valueIdScope.TryGetValue(out var value) ? value
            : valueIdScope.GetValue(this, TypeMemberDeclarationsAspect.Invocable_ValueIdScope);

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

    public IFlowState FlowStateBefore()
        => TypeMemberDeclarationsAspect.ConcreteInvocable_FlowStateBefore(this);

    internal override IPreviousValueId PreviousValueId(IChildNode before, IInheritanceContext ctx)
        => TypeMemberDeclarationsAspect.Invocable_PreviousValueId(this);

    internal override IFlowState InheritedFlowStateBefore(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == Body)
            return Parameters.LastOrDefault()?.FlowStateAfter ?? FlowStateBefore();
        if (Parameters.IndexOf(child) is int index)
        {
            if (index == 0)
                return FlowStateBefore();
            return Parameters[index - 1].FlowStateAfter;
        }

        return base.InheritedFlowStateBefore(child, descendant, ctx);
    }
}
