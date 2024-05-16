using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FieldDefinitionNode : TypeMemberDefinitionNode, IFieldDefinitionNode
{
    public override IFieldDefinitionSyntax Syntax { get; }
    public override UserTypeSymbol ContainingSymbol => (UserTypeSymbol)base.ContainingSymbol;
    public bool IsMutableBinding => Syntax.IsMutableBinding;
    public override IdentifierName Name => Syntax.Name;
    StandardName INamedDeclarationNode.Name => Name;
    public ITypeNode TypeNode { get; }
    private ValueAttribute<DataType> type;
    public DataType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, TypeMemberDeclarationsAspect.FieldDeclaration_Type);
    public override LexicalScope LexicalScope => throw new NotImplementedException();
    private ValueAttribute<FieldSymbol> symbol;
    public override FieldSymbol Symbol
        => symbol.TryGetValue(out var value) ? value
            : symbol.GetValue(this, SymbolAttribute.FieldDeclaration);
    public IUntypedExpressionNode? Initializer { get; }

    public FieldDefinitionNode(IFieldDefinitionSyntax syntax, ITypeNode type, IUntypedExpressionNode? initializer)
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
