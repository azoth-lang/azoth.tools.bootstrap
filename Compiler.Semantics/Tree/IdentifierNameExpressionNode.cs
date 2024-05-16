using System;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class IdentifierNameExpressionNode : NameExpressionNode, IIdentifierNameExpressionNode
{
    protected override bool MayHaveRewrite => true;
    public override IIdentifierNameExpressionSyntax Syntax { get; }
    public IdentifierName? Name => Syntax.Name;
    public Symbol? ReferencedSymbol => throw new NotImplementedException();

    public IdentifierNameExpressionNode(IIdentifierNameExpressionSyntax syntax)
    {
        Syntax = syntax;
    }

    protected override INameExpressionNode Rewrite()
    {
        INameExpressionNode result;
        result = MissingNameExpressionRewrite.IdentifierNameExpressionNode_Rewrite(this);
        if (!ReferenceEquals(result, this))
            return result;

        return this;
    }
}
