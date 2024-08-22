using System;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;

public sealed class SymbolTypeSyntax : TypeSyntax
{
    public SymbolSyntax Symbol { get; }

    public SymbolTypeSyntax(SymbolSyntax symbol)
        : base(symbol)
    {
        Symbol = symbol;
    }

    #region Equality
    public override bool Equals(TypeSyntax? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is SymbolTypeSyntax type
               && Symbol.Equals(type.Symbol);
    }

    public override int GetHashCode() => HashCode.Combine(Symbol);
    #endregion

    public override string ToString() => Symbol.ToString();
}
