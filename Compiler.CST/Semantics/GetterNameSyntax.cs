using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Semantics;

public sealed class GetterNameSyntax : SyntaxSemantics, IMemberAccessSyntaxSemantics
{
    public Promise<MethodSymbol> Symbol { get; }
    IPromise<Symbol?> IMemberAccessSyntaxSemantics.Symbol => Symbol;
    public override Promise<DataType> Type { get; } = new();

    public GetterNameSyntax(MethodSymbol symbol)
        : base(symbol)
    {
        Symbol = Promise.ForValue(symbol);
    }
}
