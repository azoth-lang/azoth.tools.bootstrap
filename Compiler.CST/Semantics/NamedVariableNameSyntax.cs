using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Semantics;

public sealed class NamedVariableNameSyntax : VariableNameSyntax, IIdentifierNameExpressionSyntaxSemantics
{
    public override NamedVariableSymbol Symbol => symbolPromise.Result;
    private readonly IPromise<NamedVariableSymbol> symbolPromise;
    IPromise<Symbol?> IIdentifierNameExpressionSyntaxSemantics.Symbol => symbolPromise;
    private IFixedSet<Symbol>? symbols;
    public override IFixedSet<Symbol> Symbols => symbols ??= FixedSet.Create(symbolPromise.Result);

    public NamedVariableNameSyntax(NamedVariableSymbol symbol) : base(symbol)
    {
        symbolPromise = Promise.ForValue(symbol);
    }

    public NamedVariableNameSyntax(IPromise<NamedVariableSymbol> symbolPromise)
        : base(FixedSet.Empty<NamedVariableSymbol>())
    {
        this.symbolPromise = symbolPromise;
    }
}
