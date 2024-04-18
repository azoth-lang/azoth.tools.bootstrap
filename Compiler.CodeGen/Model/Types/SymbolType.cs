using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

public sealed class SymbolType : Type
{
    public static SymbolType? Create(Symbol? symbol)
        => symbol is null ? null : new SymbolType(symbol);

    public static SymbolType CreateFromSyntax(Grammar grammar, SymbolNode syntax)
        => new(Symbol.CreateFromSyntax(grammar, syntax));

    public static SymbolType CreateExternalFromSyntax(SymbolNode syntax)
        => new(Symbol.CreateExternalFromSyntax(syntax));

    public Symbol Symbol { get; }

    public SymbolType(Symbol symbol)
        : base(symbol)
    {
        Symbol = symbol;
    }

    #region Equality
    public override bool Equals(Type? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is SymbolType type
               && Symbol.Equals(type.Symbol);
    }

    public override int GetHashCode() => HashCode.Combine(Symbol);
    #endregion

    public override int GetEquivalenceHashCode() => Symbol.GetEquivalenceHashCode();

    public override SymbolType WithSymbol(Symbol symbol) => new(symbol);

    public override string ToString() => Symbol.ToString();
}
