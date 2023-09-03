using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class SelfExpressionSyntax : ExpressionSyntax, ISelfExpressionSyntax
{
    public bool IsImplicit { get; }
    public Promise<SelfParameterSymbol?> ReferencedSymbol { get; } = new Promise<SelfParameterSymbol?>();

    public SelfExpressionSyntax(TextSpan span, bool isImplicit)
        : base(span)
    {
        IsImplicit = isImplicit;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString()
    {
        return IsImplicit ? "⟦self⟧" : "self";
    }
}
