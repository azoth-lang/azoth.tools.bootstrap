using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Semantics;

public sealed class TypeNameSyntax : SyntaxSemantics, IIdentifierNameExpressionSyntaxSemantics,
    IMemberAccessSyntaxSemantics
{
    public TypeSymbol Symbol { get; }
    private readonly IPromise<TypeSymbol> symbolPromise;
    IPromise<Symbol> IMemberAccessSyntaxSemantics.Symbol => symbolPromise;
    IPromise<Symbol> IIdentifierNameExpressionSyntaxSemantics.Symbol => symbolPromise;

    public TypeNameSyntax(TypeSymbol symbol) : base(symbol)
    {
        Symbol = symbol;
        symbolPromise = Promise.ForValue(Symbol);
    }
}
