using System;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FieldDefinitionNode : TypeMemberDefinitionNode, IFieldDefinitionNode
{
    public override IFieldDefinitionSyntax Syntax { get; }
    public override UserTypeSymbol ContainingSymbol => (UserTypeSymbol)base.ContainingSymbol;
    bool IBindingNode.IsLentBinding => false;
    public bool IsMutableBinding => Syntax.IsMutableBinding;
    public override IdentifierName Name => Syntax.Name;
    TypeName INamedDeclarationNode.Name => Name;
    public ITypeNode TypeNode { get; }
    private ValueAttribute<IMaybeAntetype> bindingAntetype;
    public IMaybeAntetype BindingAntetype
        => bindingAntetype.TryGetValue(out var value) ? value
            : bindingAntetype.GetValue(this, DeclarationsAntetypesAspect.FieldDefinition_BindingAntetype);
    private DataType? bindingType;
    private bool bindingTypeCached;
    public DataType BindingType
        => GrammarAttribute.IsCached(in bindingTypeCached) ? bindingType!
            : this.Synthetic(ref bindingTypeCached, ref bindingType, TypeMemberDeclarationsAspect.FieldDeclaration_BindingType);
    public override LexicalScope LexicalScope => throw new NotImplementedException();
    private ValueAttribute<FieldSymbol> symbol;
    public override FieldSymbol Symbol
        => symbol.TryGetValue(out var value) ? value
            : symbol.GetValue(this, SymbolAspect.FieldDefinition_Symbol);
    public IAmbiguousExpressionNode? Initializer { get; }
    private ValueAttribute<ValueIdScope> valueIdScope;
    public ValueIdScope ValueIdScope
        => valueIdScope.TryGetValue(out var value) ? value
            : valueIdScope.GetValue(this, TypeMemberDeclarationsAspect.FieldDefinition_ValueIdScope);
    public ValueId BindingValueId => throw new NotImplementedException();

    public FieldDefinitionNode(IFieldDefinitionSyntax syntax, ITypeNode type, IAmbiguousExpressionNode? initializer)
    {
        Syntax = syntax;
        TypeNode = Child.Attach(this, type);
        Initializer = Child.Attach(this, initializer);
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        TypeMemberDeclarationsAspect.FieldDeclaration_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
