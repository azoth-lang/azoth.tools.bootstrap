using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Semantics;

public sealed class SpecialTypeNameExpressionSyntaxSemantics : SyntaxSemantics
{
    public TypeSymbol Symbol { get; }

    public SpecialTypeNameExpressionSyntaxSemantics(TypeSymbol symbol)
        : base(symbol)
    {
        Symbol = symbol;
    }
}
