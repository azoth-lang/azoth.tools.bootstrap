using System;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FieldDeclarationNode : TypeMemberDeclarationNode, IFieldDeclarationNode
{
    public override IFieldDeclarationSyntax Syntax { get; }
    public IdentifierName Name => Syntax.Name;
    public ITypeNode Type { get; }
    public override LexicalScope LexicalScope => throw new NotImplementedException();
    public override IDeclarationSymbolNode SymbolNode => throw new NotImplementedException();

    public FieldDeclarationNode(IFieldDeclarationSyntax syntax, ITypeNode type)
    {
        Syntax = syntax;
        Type = Child.Attach(this, type);
    }
}
