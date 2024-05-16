using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class UntypedExpressionNode : CodeNode, IUntypedExpressionNode
{
    public abstract override IExpressionSyntax Syntax { get; }

    protected override IUntypedExpressionNode Rewrite() => RewriteNotSupported<IUntypedExpressionNode>();
}
