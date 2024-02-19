using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class MethodInvocationExpression : Expression, IMethodInvocationExpression
{
    public IExpression Context { get; }
    public MethodSymbol ReferencedSymbol { get; }
    public IFixedList<IExpression> Arguments { get; }

    public MethodInvocationExpression(
        TextSpan span,
        DataType dataType,
        ExpressionSemantics semantics,
        IExpression context,
        MethodSymbol referencedSymbol,
        IFixedList<IExpression> arguments)
        : base(span, dataType, semantics)
    {
        Context = context;
        ReferencedSymbol = referencedSymbol;
        Arguments = arguments;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString()
        => $"{Context.ToGroupedString(ExpressionPrecedence)}.{ReferencedSymbol.Name}({string.Join(", ", Arguments)})";
}
