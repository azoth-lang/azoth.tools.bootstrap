using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal class SelfExpression : Expression, ISelfExpression
{
    public SelfParameterSymbol ReferencedSymbol { get; }
    public bool IsImplicit { get; }

    public SelfExpression(TextSpan span, DataType dataType, SelfParameterSymbol referencedSymbol, bool isImplicit)
        : base(span, dataType)
    {
        ReferencedSymbol = referencedSymbol;
        IsImplicit = isImplicit;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => IsImplicit ? "⟦self⟧" : "self";
}
