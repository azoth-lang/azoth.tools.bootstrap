using System;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FunctionDeclarationNode : CodeNode, IFunctionDeclarationNode
{
    public override IFunctionDeclarationSyntax Syntax { get; }

    public NamespaceSymbol ContainingNamespace => throw new NotImplementedException();

    public FunctionDeclarationNode(IFunctionDeclarationSyntax syntax)
    {
        Syntax = syntax;
    }
}
