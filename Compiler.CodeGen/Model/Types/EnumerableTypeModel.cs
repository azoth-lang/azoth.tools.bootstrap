using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

public sealed class EnumerableTypeModel : CollectionTypeModel
{
    public override bool IsValueType => false;

    internal EnumerableTypeModel(TypeModel elementType)
        : base(elementType) { }

    #region Equality
    public override bool Equals(TypeModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is EnumerableTypeModel type && ElementType.Equals(type.ElementType);
    }

    public override int GetHashCode() => HashCode.Combine(ElementType, typeof(EnumerableTypeModel));
    #endregion

    public override EnumerableTypeModel WithOptionalSymbol(Symbol symbol)
        => new EnumerableTypeModel(ElementType.WithOptionalSymbol(symbol));

    public override bool IsSubtypeOf(TypeModel other)
    {
        if (other is OptionalTypeModel optionalType) return IsSubtypeOf(optionalType.UnderlyingType);
        return other is EnumerableTypeModel enumerableType && ElementType.IsSubtypeOf(enumerableType.ElementType);
    }
    public override string ToString() => $"IEnumerable<{ElementType}>";
}
