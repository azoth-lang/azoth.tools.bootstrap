using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal sealed class VariableNameExpression : Expression, IVariableNameExpression
{
    public NamedVariableSymbol ReferencedSymbol { get; }
    public bool IsMove { get; }

    public VariableNameExpression(TextSpan span, DataType dataType, NamedVariableSymbol referencedSymbol, bool isMove)
        : base(span, dataType)
    {
        ReferencedSymbol = referencedSymbol;
        IsMove = isMove;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => ReferencedSymbol.Name.ToString();
}
