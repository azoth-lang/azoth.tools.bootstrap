using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
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
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public override LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.FunctionDefinition_LexicalScope,
                ReferenceEqualityComparer.Instance);
    private ValueAttribute<FunctionSymbol> symbol;
    public override FunctionSymbol Symbol
        => symbol.TryGetValue(out var value) ? value
            : symbol.GetValue(this, SymbolAspect.FunctionDefinition_Symbol);
    public IFixedList<INamedParameterNode> Parameters { get; }
    IFixedList<IConstructorOrInitializerParameterNode> IInvocableDefinitionNode.Parameters => Parameters;
    public ITypeNode? Return { get; }
    public IBodyNode Body { get; }
    private FunctionType? type;
    private bool typeCached;
    public FunctionType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, TypeMemberDeclarationsAspect.FunctionDeclaration_Type);
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

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == Body)
            return LexicalScope;
        return base.InheritedContainingLexicalScope(child, descendant, ctx);
    }

    public IFlowState FlowStateBefore()
        => TypeMemberDeclarationsAspect.ConcreteInvocable_FlowStateBefore(this);

    internal override IPreviousValueId PreviousValueId(IChildNode before, IInheritanceContext ctx)
        => TypeMemberDeclarationsAspect.Invocable_PreviousValueId(this);

    internal override IFlowState InheritedFlowStateBefore(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
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

    internal override IMaybeExpressionAntetype? InheritedExpectedAntetype(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == Body) return Type.Return.Type.ToAntetype();
        return base.InheritedExpectedAntetype(child, descendant, ctx);
    }
}
