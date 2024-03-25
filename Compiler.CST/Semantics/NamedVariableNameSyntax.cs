using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Semantics;

public sealed class NamedVariableNameSyntax : VariableNameSyntax, IIdentifierNameExpressionSyntaxSemantics
{
    public override NamedVariableSymbol Symbol { get; }
    private readonly Promise<NamedVariableSymbol> symbolPromise;
    IPromise<Symbol?> IIdentifierNameExpressionSyntaxSemantics.Symbol => symbolPromise;

    public NamedVariableNameSyntax(NamedVariableSymbol symbol) : base(symbol)
    {
        Symbol = symbol;
        symbolPromise = Promise.ForValue(symbol);
    }
}
