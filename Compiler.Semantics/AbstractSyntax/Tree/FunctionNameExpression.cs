using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal class FunctionNameExpression : Expression, IFunctionNameExpression
{
    public FunctionSymbol ReferencedSymbol { get; }

    public FunctionNameExpression(TextSpan span, DataType dataType, FunctionSymbol referencedSymbol)
        : base(span, dataType)
    {
        ReferencedSymbol = referencedSymbol;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => ReferencedSymbol.Name.ToString();
}
