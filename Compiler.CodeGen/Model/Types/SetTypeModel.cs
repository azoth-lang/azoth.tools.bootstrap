using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

public sealed class SetTypeModel : CollectionTypeModel
{
    public override bool IsValueType => false;

    internal SetTypeModel(TypeModel underlyingType)
        : base(underlyingType) { }

    #region Equality
    public override bool Equals(TypeModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is SetTypeModel type && ElementType.Equals(type.ElementType);
    }

    public override int GetHashCode() => HashCode.Combine(ElementType, typeof(SetTypeModel));
    #endregion

    public override SetTypeModel WithSymbol(Symbol symbol)
        => new SetTypeModel(ElementType.WithSymbol(symbol));

    public override bool IsSubtypeOf(TypeModel other)
    {
        if (other is OptionalType optionalType) return IsSubtypeOf(optionalType.UnderlyingType);
        return other is SetTypeModel setType && ElementType.IsSubtypeOf(setType.ElementType);
    }

    public override string ToString() => $"{{{ElementType}}}";
}
