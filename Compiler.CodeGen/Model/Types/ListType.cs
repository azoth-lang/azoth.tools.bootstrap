using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

public sealed class ListType : CollectionType
{
    public override bool IsValueType => false;

    internal ListType(TypeModel elementType)
        : base(elementType) { }

    #region Equality
    public override bool Equals(TypeModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ListType type
               && ElementType.Equals(type.ElementType);
    }

    public override int GetHashCode() => HashCode.Combine(ElementType, typeof(ListType));
    #endregion

    public override ListType WithSymbol(Symbol symbol)
        => new ListType(ElementType.WithSymbol(symbol));

    public override bool IsSubtypeOf(TypeModel other)
    {
        if (other is OptionalType optionalType) return IsSubtypeOf(optionalType.UnderlyingType);
        return other is ListType listType && ElementType.IsSubtypeOf(listType.ElementType);
    }

    public override string ToString() => ElementType + "*";
}
