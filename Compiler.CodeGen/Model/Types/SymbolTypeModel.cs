using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

public sealed class SymbolTypeModel : NonOptionalTypeModel
{
    public static SymbolTypeModel? Create(Symbol? symbol)
        => symbol is null ? null : new SymbolTypeModel(symbol);

    public static SymbolTypeModel CreateFromSyntax(TreeModel tree, SymbolSyntax syntax)
        => new(Symbol.CreateFromSyntax(tree, syntax));

    public Symbol Symbol { get; }
    public override bool IsValueType => Symbol.IsValueType;

    public SymbolTypeModel(Symbol symbol)
        : base(symbol)
    {
        Symbol = symbol;
    }

    #region Equality
    public override bool Equals(TypeModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is SymbolTypeModel type
               && Symbol.Equals(type.Symbol);
    }

    public override int GetHashCode() => HashCode.Combine(Symbol);
    #endregion

    public override OptionalTypeModel WithOptionalSymbol(Symbol symbol)
        => new(new SymbolTypeModel(symbol));

    public override bool IsSubtypeOf(TypeModel other)
    {
        if (other is OptionalTypeModel optionalType)
            return IsSubtypeOf(optionalType.UnderlyingType);
        if (other is not SymbolTypeModel type) return false;
        if (Symbol.Equals(type.Symbol)) return true;
        if (Symbol is not InternalSymbol symbol
            || type.Symbol is not InternalSymbol otherSymbol) return false;
        bool isSubtypeOf = symbol.ReferencedNode.AncestorNodes
                                     .Contains(otherSymbol.ReferencedNode);
        return isSubtypeOf;
    }

    public override string ToString() => Symbol.ToString();
}
