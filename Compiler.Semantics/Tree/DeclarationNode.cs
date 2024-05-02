using System;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

public abstract class DeclarationNode : CodeNode, IDeclarationNode
{
    public abstract override IDeclarationSyntax Syntax { get; }

    public NamespaceSymbol? ContainingNamespace => throw new NotImplementedException();
}
