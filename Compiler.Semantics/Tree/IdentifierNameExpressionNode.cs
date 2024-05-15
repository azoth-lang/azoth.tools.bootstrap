using System;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class IdentifierNameExpressionNode : NameExpressionNode, IIdentifierNameExpressionNode
{
    public override IIdentifierNameExpressionSyntax Syntax { get; }
    public IdentifierName? Name => Syntax.Name;
    public Symbol? ReferencedSymbol => throw new NotImplementedException();

    public IdentifierNameExpressionNode(IIdentifierNameExpressionSyntax syntax)
    {
        Syntax = syntax;
    }
}
