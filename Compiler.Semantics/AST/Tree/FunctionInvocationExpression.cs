using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class FunctionInvocationExpression : Expression, IFunctionInvocationExpression
{
    public FunctionSymbol ReferencedSymbol { get; }
    public IFixedList<IExpression> Arguments { get; }

    public FunctionInvocationExpression(
        TextSpan span,
        DataType dataType,
        FunctionSymbol referencedSymbol,
        IFixedList<IExpression> arguments)
        : base(span, dataType)
    {
        ReferencedSymbol = referencedSymbol;
        Arguments = arguments;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString()
        => $"{ReferencedSymbol.ContainingSymbol}.{ReferencedSymbol.Name}({string.Join(", ", Arguments)})";
}
