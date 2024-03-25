using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Semantics;

public sealed class SelfExpressionSyntax : VariableNameSyntax, ISelfExpressionSyntaxSemantics
{
    public override SelfParameterSymbol Symbol { get; }
    public Promise<Pseudotype> Pseudotype { get; } = new();
    IPromise<Pseudotype> ISelfExpressionSyntaxSemantics.Pseudotype => Pseudotype;

    public SelfExpressionSyntax(SelfParameterSymbol symbol)
        : base(symbol)
    {
        Symbol = symbol;
    }
}
