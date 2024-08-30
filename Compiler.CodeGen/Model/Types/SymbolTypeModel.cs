using System;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

public sealed class SymbolTypeModel : NonOptionalTypeModel
{
    [return: NotNullIfNotNull(nameof(symbol))]
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
        switch ((Symbol, type.Symbol))
        {
            case (InternalSymbol symbol, InternalSymbol otherSymbol):
                return symbol.ReferencedNode.AncestorNodes
                    .Contains(otherSymbol.ReferencedNode);
            case (ExternalSymbol symbol, ExternalSymbol otherSymbol):
                return symbol.TypeDeclaration?.Supertypes.Contains(otherSymbol) ?? false;
            default:
                return false;
        }
    }

    public override string ToString() => Symbol.ToString();
}
