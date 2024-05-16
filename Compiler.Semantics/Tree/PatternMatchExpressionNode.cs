using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class PatternMatchExpressionNode : ExpressionNode, IPatternMatchExpressionNode
{
    public override IPatternMatchExpressionSyntax Syntax { get; }
    private Child<IUntypedExpressionNode> referent;
    public IUntypedExpressionNode Referent => referent.Value;
    public IPatternNode Pattern { get; }

    public PatternMatchExpressionNode(
        IPatternMatchExpressionSyntax syntax,
        IUntypedExpressionNode referent,
        IPatternNode pattern)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
        Pattern = Child.Attach(this, pattern);
    }
}