using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

public sealed class OptionalType : NonVoidType
{
    public NonVoidType UnderlyingType { get; }

    public OptionalType(NonVoidType underlyingType)
        : base(underlyingType.UnderlyingSymbol)
    {
        UnderlyingType = underlyingType;
    }

    #region Equality
    public override bool Equals(Type? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is OptionalType type
               && UnderlyingType.Equals(type.UnderlyingType);
    }

    public override int GetHashCode() => HashCode.Combine(UnderlyingType, typeof(OptionalType));
    #endregion

    public override int GetEquivalenceHashCode()
        => HashCode.Combine(UnderlyingType.GetEquivalenceHashCode(), typeof(OptionalType));

    public override OptionalType WithSymbol(Symbol symbol)
        => new OptionalType(UnderlyingType.WithSymbol(symbol));

    public override bool IsSubtypeOf(Type other)
        => other is OptionalType optionalType && UnderlyingType.IsSubtypeOf(optionalType.UnderlyingType);

    public override string ToString() => UnderlyingType + "?";
}
