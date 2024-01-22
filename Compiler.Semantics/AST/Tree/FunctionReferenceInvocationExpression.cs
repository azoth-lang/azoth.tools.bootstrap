using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class FunctionReferenceInvocationExpression : Expression, IFunctionReferenceInvocationExpression
{
    public NamedBindingSymbol ReferencedSymbol { get; }
    Symbol IInvocationExpression.ReferencedSymbol => ReferencedSymbol;
    public FixedList<IExpression> Arguments { get; }

    public FunctionReferenceInvocationExpression(
        TextSpan span,
        DataType dataType,
        ExpressionSemantics semantics,
        NamedBindingSymbol referencedSymbol,
        FixedList<IExpression> arguments)
        : base(span, dataType, semantics)
    {
        ReferencedSymbol = referencedSymbol;
        Arguments = arguments;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString()
        => $"{ReferencedSymbol.Name}({string.Join(", ", Arguments)})";
}
