using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

public sealed class ListTypeModel : CollectionTypeModel
{
    public override bool IsValueType => false;

    internal ListTypeModel(TypeModel elementType)
        : base(elementType) { }

    #region Equality
    public override bool Equals(TypeModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ListTypeModel type
               && ElementType.Equals(type.ElementType);
    }

    public override int GetHashCode() => HashCode.Combine(ElementType, typeof(ListTypeModel));
    #endregion

    public override ListTypeModel WithOptionalSymbol(Symbol symbol)
        => new ListTypeModel(ElementType.WithOptionalSymbol(symbol));

    public override bool IsSubtypeOf(TypeModel other)
    {
        if (other is OptionalTypeModel optionalType) return IsSubtypeOf(optionalType.UnderlyingType);
        return other is ListTypeModel listType && ElementType.IsSubtypeOf(listType.ElementType);
    }

    public override string ToString() => ElementType + "*";
}
