using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class FunctionReferenceInvocationExpression : Expression, IFunctionReferenceInvocationExpression
{
    public IExpression Referent { get; }
    public FixedList<IExpression> Arguments { get; }

    public FunctionReferenceInvocationExpression(
        TextSpan span,
        DataType dataType,
        ExpressionSemantics semantics,
        IExpression referent,
        FixedList<IExpression> arguments)
        : base(span, dataType, semantics)
    {
        Referent = referent;
        Arguments = arguments;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString()
        => $"{Referent.ToGroupedString(ExpressionPrecedence)}({string.Join(", ", Arguments)})";
}
