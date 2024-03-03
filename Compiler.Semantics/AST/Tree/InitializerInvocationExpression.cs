using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal sealed class InitializerInvocationExpression : Expression, IInitializerInvocationExpression
{
    public InitializerSymbol ReferencedSymbol { get; }
    public IFixedList<IExpression> Arguments { get; }

    public InitializerInvocationExpression(
        TextSpan span,
        DataType dataType,
        ExpressionSemantics semantics,
        InitializerSymbol referencedSymbol,
        IFixedList<IExpression> arguments)
        : base(span, dataType, semantics)
    {
        ReferencedSymbol = referencedSymbol;
        Arguments = arguments;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString()
    {
        var name = ReferencedSymbol.InitializerName is null ? $".{ReferencedSymbol.InitializerName}" : "";
        return $"{ReferencedSymbol.ContextTypeSymbol}::init{name}({string.Join(", ", Arguments)})";
    }
}
