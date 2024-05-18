using System;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class SelfExpressionNode : AmbiguousNameExpressionNode, ISelfExpressionNode
{
    public override ISelfExpressionSyntax Syntax { get; }
    public bool IsImplicit => Syntax.IsImplicit;
    public Pseudotype Pseudotype => throw new NotImplementedException();

    public SelfExpressionNode(ISelfExpressionSyntax syntax)
    {
        Syntax = syntax;
    }
}
