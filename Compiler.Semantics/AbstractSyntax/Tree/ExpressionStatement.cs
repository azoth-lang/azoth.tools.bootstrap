using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal class ExpressionStatement : Statement, IExpressionStatement
{
    public IExpression Expression { [DebuggerStepThrough] get; }

    public ExpressionStatement(TextSpan span, IExpression expression)
        : base(span)
    {
        Expression = expression;
    }

    public override string ToString() => Expression + ";";
}
