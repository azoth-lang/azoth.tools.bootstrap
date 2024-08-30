using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FieldDefinitionNode : TypeMemberDefinitionNode, IFieldDefinitionNode
{
    public override IFieldDefinitionSyntax Syntax { get; }
    public override IUserTypeDeclarationNode ContainingDeclaration
        => (IUserTypeDeclarationNode)base.ContainingDeclaration;
    public override UserTypeSymbol ContainingSymbol => (UserTypeSymbol)base.ContainingSymbol;
    bool IBindingNode.IsLentBinding => false;
    public bool IsMutableBinding => Syntax.IsMutableBinding;
    public override IdentifierName Name => Syntax.Name;
    TypeName INamedDeclarationNode.Name => Name;
    public ITypeNode TypeNode { get; }
    private IMaybeAntetype? bindingAntetype;
    private bool bindingAntetypeCached;
    public IMaybeAntetype BindingAntetype
        => GrammarAttribute.IsCached(in bindingAntetypeCached) ? bindingAntetype!
            : this.Synthetic(ref bindingAntetypeCached, ref bindingAntetype,
                DeclarationsAntetypesAspect.FieldDefinition_BindingAntetype);
    private DataType? bindingType;
    private bool bindingTypeCached;
    public DataType BindingType
        => GrammarAttribute.IsCached(in bindingTypeCached) ? bindingType!
            : this.Synthetic(ref bindingTypeCached, ref bindingType,
                TypeMemberDeclarationsAspect.FieldDefinition_BindingType);
    public override LexicalScope LexicalScope => throw new NotImplementedException();
    private FieldSymbol? symbol;
    private bool symbolCached;
    public override FieldSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol, SymbolsAspect.FieldDefinition_Symbol);
    private RewritableChild<IAmbiguousExpressionNode?> initializer;
    private bool initializerCached;
    public IAmbiguousExpressionNode? TempInitializer
        => GrammarAttribute.IsCached(in initializerCached) ? initializer.UnsafeValue
            : this.RewritableChild(ref initializerCached, ref initializer);
    public IAmbiguousExpressionNode? CurrentInitializer => initializer.UnsafeValue;
    public IExpressionNode? Initializer => TempInitializer as IExpressionNode;
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.FieldDefinition_ValueIdScope);
    public ValueId BindingValueId => throw new NotImplementedException();
    public IEntryNode Entry { get; }
    public IExitNode Exit { get; }
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.FieldDefinition_VariableBindingsMap);

    public FieldDefinitionNode(IFieldDefinitionSyntax syntax, ITypeNode type, IAmbiguousExpressionNode? initializer)
    {
        Syntax = syntax;
        TypeNode = Child.Attach(this, type);
        this.initializer = Child.Create(this, initializer);
        Entry = Child.Attach(this, new EntryNode());
        Exit = Child.Attach(this, new ExitNode());
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        TypeMemberDeclarationsAspect.FieldDefinition_ContributeDiagnostics(this, diagnostics);
        base.Contribute_Diagnostics(diagnostics);
    }

    internal override DataType? Inherited_ExpectedReturnType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (child == CurrentInitializer)
            // Null is the signal that this is a field initializer and not a method body
            return null;
        return base.Inherited_ExpectedReturnType(child, descendant, ctx);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(
        SemanticNode child,
        SemanticNode descendant,
        IInheritanceContext ctx)
    {
        if (descendant == Entry) return ControlFlowSet.CreateNormal(Initializer ?? (IControlFlowNode)Exit);
        if (descendant == CurrentInitializer) return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(
        SemanticNode child,
        SemanticNode descendant,
        IInheritanceContext ctx)
        => VariableBindingsMap;

    internal override IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => Entry;
}
