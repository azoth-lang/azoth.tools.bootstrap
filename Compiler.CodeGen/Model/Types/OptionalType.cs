using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

public sealed class OptionalType : TypeModel
{
    public TypeModel UnderlyingType { get; }
    public override bool IsValueType => UnderlyingType.IsValueType;

    public OptionalType(TypeModel underlyingType)
        : base(underlyingType.UnderlyingSymbol)
    {
        UnderlyingType = underlyingType;
    }

    #region Equality
    public override bool Equals(TypeModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is OptionalType type
               && UnderlyingType.Equals(type.UnderlyingType);
    }

    public override int GetHashCode() => HashCode.Combine(UnderlyingType, typeof(OptionalType));
    #endregion

    public override OptionalType WithSymbol(Symbol symbol)
        => new OptionalType(UnderlyingType.WithSymbol(symbol));

    public override bool IsSubtypeOf(TypeModel other)
        => other is OptionalType optionalType && UnderlyingType.IsSubtypeOf(optionalType.UnderlyingType);

    public override NonOptionalType ToNonOptional() => UnderlyingType.ToNonOptional();

    public override string ToString() => UnderlyingType + "?";
}
