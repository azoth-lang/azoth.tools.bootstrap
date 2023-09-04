using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class ResultStatement : Statement, IResultStatement
{
    public IExpression Expression { get; }

    public ResultStatement(TextSpan span, IExpression expression)
        : base(span)
    {
        Expression = expression;
    }

    public override string ToString() => $"=> {Expression};";
}
