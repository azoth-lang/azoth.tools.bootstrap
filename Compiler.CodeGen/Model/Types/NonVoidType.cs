using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

[Closed(typeof(SymbolType), typeof(CollectionType), typeof(OptionalType))]
public abstract class NonVoidType : Type
{
    public Symbol UnderlyingSymbol { get; }

    private protected NonVoidType(Symbol underlyingSymbol)
    {
        UnderlyingSymbol = underlyingSymbol;
    }

    public abstract override NonVoidType WithSymbol(Symbol symbol);
}
