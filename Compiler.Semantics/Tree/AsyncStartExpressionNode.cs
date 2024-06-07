using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AsyncStartExpressionNode : ExpressionNode, IAsyncStartExpressionNode
{
    public override IAsyncStartExpressionSyntax Syntax { get; }
    public bool Scheduled => Syntax.Scheduled;
    private Child<IAmbiguousExpressionNode> expression;
    public IAmbiguousExpressionNode Expression => expression.Value;
    public IExpressionNode FinalExpression => (IExpressionNode)expression.FinalValue;
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.AsyncStartExpression_Antetype);

    public AsyncStartExpressionNode(IAsyncStartExpressionSyntax syntax, IAmbiguousExpressionNode expression)
    {
        Syntax = syntax;
        this.expression = Child.Create(this, expression);
    }
}
