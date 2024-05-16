using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class MissingNameExpressionNode : NameExpressionNode, IMissingNameExpressionNode
{
    public override IIdentifierNameExpressionSyntax Syntax { get; }

    public MissingNameExpressionNode(IIdentifierNameExpressionSyntax syntax)
    {
        Requires.That(nameof(syntax), syntax.Name is null, "Name must be null.");
        Syntax = syntax;
    }
}
