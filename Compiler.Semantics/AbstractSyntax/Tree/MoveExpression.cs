using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal class MoveExpression : Expression, IMoveExpression
{
    public BindingSymbol ReferencedSymbol { get; }
    public IExpression Referent { get; }

    public MoveExpression(
        TextSpan span, DataType dataType, BindingSymbol referencedSymbol, IExpression referent)
        : base(span, dataType)
    {
        ReferencedSymbol = referencedSymbol;
        Referent = referent;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

    public override string ToString() => $"move {Referent}";
}
