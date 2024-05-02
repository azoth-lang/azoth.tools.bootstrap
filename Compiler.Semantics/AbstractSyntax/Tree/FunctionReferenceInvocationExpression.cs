using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal class FunctionReferenceInvocationExpression : Expression, IFunctionReferenceInvocationExpression
{
    public IExpression Referent { get; }
    public IFixedList<IExpression> Arguments { get; }

    public FunctionReferenceInvocationExpression(
        TextSpan span,
        DataType dataType,
        IExpression referent,
        IFixedList<IExpression> arguments)
        : base(span, dataType)
    {
        Referent = referent;
        Arguments = arguments;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString()
        => $"{Referent.ToGroupedString(ExpressionPrecedence)}({string.Join(", ", Arguments)})";
}
