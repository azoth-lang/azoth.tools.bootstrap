using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

public sealed class SymbolType : NonOptionalType
{
    public static SymbolType? Create(Symbol? symbol)
        => symbol is null ? null : new SymbolType(symbol);

    public static SymbolType CreateFromSyntax(TreeModel tree, SymbolSyntax syntax)
        => new(Symbol.CreateFromSyntax(tree, syntax));

    public Symbol Symbol { get; }

    public SymbolType(Symbol symbol)
        : base(symbol)
    {
        Symbol = symbol;
    }

    #region Equality
    public override bool Equals(TypeModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is SymbolType type
               && Symbol.Equals(type.Symbol);
    }

    public override int GetHashCode() => HashCode.Combine(Symbol);
    #endregion

    public override SymbolType WithSymbol(Symbol symbol) => new(symbol);

    public override bool IsSubtypeOf(TypeModel other)
    {
        if (other is OptionalType optionalType)
            return IsSubtypeOf(optionalType.UnderlyingType);
        if (other is not SymbolType type) return false;
        if (Symbol.Equals(type.Symbol)) return true;
        if (Symbol is not InternalSymbol symbol
            || type.Symbol is not InternalSymbol otherSymbol) return false;
        bool isSubtypeOf = symbol.ReferencedNode.AncestorNodes
                                     .Contains(otherSymbol.ReferencedNode);
        return isSubtypeOf;
    }

    public override string ToString() => Symbol.ToString();
}
