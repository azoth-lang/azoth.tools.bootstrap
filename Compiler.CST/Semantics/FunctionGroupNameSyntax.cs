using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Semantics;

public sealed class FunctionGroupNameSyntax : SyntaxSemantics, IIdentifierNameExpressionSyntaxSemantics,
    IMemberAccessSyntaxSemantics
{
    public new IFixedSet<FunctionSymbol> Symbols { get; }
    public Promise<FunctionSymbol?> Symbol { get; } = new();
    IPromise<Symbol?> IMemberAccessSyntaxSemantics.Symbol => Symbol;
    IPromise<Symbol?> IIdentifierNameExpressionSyntaxSemantics.Symbol => Symbol;
    public override IPromise<FunctionType?> Type { get; }

    public FunctionGroupNameSyntax(IFixedSet<FunctionSymbol> symbols) : base(symbols)
    {
        Symbols = symbols;
        Type = Symbol.Select(s => s?.Type);
    }
}
