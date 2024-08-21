using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

public sealed class SetType : CollectionType
{
    public override bool IsValueType => false;

    internal SetType(TypeModel underlyingType)
        : base(underlyingType) { }

    #region Equality
    public override bool Equals(TypeModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is SetType type && ElementType.Equals(type.ElementType);
    }

    public override int GetHashCode() => HashCode.Combine(ElementType, typeof(SetType));
    #endregion

    public override SetType WithSymbol(Symbol symbol)
        => new SetType(ElementType.WithSymbol(symbol));

    public override bool IsSubtypeOf(TypeModel other)
    {
        if (other is OptionalType optionalType) return IsSubtypeOf(optionalType.UnderlyingType);
        return other is SetType setType && ElementType.IsSubtypeOf(setType.ElementType);
    }

    public override string ToString() => $"{{{ElementType}}}";
}
