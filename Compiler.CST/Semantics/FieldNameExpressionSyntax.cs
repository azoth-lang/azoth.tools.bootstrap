using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Semantics;

public sealed class FieldNameExpressionSyntax : SyntaxSemantics, IMemberAccessSyntaxSemantics
{
    public FieldSymbol Symbol { get; }
    private readonly IPromise<FieldSymbol> symbolPromise;
    IPromise<Symbol> IMemberAccessSyntaxSemantics.Symbol => symbolPromise;
    public override Promise<DataType> Type { get; } = new();

    public FieldNameExpressionSyntax(FieldSymbol symbol)
        : base(symbol)
    {
        Symbol = symbol;
        symbolPromise = Promise.ForValue(Symbol);
    }
}
