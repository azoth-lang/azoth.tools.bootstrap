using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

public sealed class SetType : CollectionType
{
    internal SetType(NonVoidType underlyingType)
        : base(underlyingType) { }

    #region Equality
    public override bool Equals(Type? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is SetType type && ElementType.Equals(type.ElementType);
    }

    public override int GetHashCode() => HashCode.Combine(ElementType, typeof(SetType));
    #endregion

    public override int GetEquivalenceHashCode()
        => HashCode.Combine(ElementType.GetEquivalenceHashCode(), typeof(SetType));

    public override SetType WithSymbol(Symbol symbol)
        => new SetType(ElementType.WithSymbol(symbol));

    public override string ToString() => $"{{{ElementType}}}";
}
