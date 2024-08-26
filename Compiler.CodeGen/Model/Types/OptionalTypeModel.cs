using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

public sealed class OptionalTypeModel : TypeModel
{
    public NonOptionalTypeModel UnderlyingType { get; }
    public override bool IsValueType => UnderlyingType.IsValueType;

    public OptionalTypeModel(NonOptionalTypeModel underlyingType)
        : base(underlyingType.UnderlyingSymbol)
    {
        UnderlyingType = underlyingType;
    }

    #region Equality
    public override bool Equals(TypeModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is OptionalTypeModel type
               && UnderlyingType.Equals(type.UnderlyingType);
    }

    public override int GetHashCode() => HashCode.Combine(UnderlyingType, typeof(OptionalTypeModel));
    #endregion

    public override OptionalTypeModel WithOptionalSymbol(Symbol symbol)
        => UnderlyingType.WithOptionalSymbol(symbol).ToOptional();

    public override bool IsSubtypeOf(TypeModel other)
        => other is OptionalTypeModel optionalType && UnderlyingType.IsSubtypeOf(optionalType.UnderlyingType);

    public override NonOptionalTypeModel ToNonOptional() => UnderlyingType;

    public override OptionalTypeModel ToOptional() => this;

    public override string ToString() => UnderlyingType + "?";
}
