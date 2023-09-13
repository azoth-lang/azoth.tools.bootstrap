using System;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class GenericParameterTypeSymbol : TypeSymbol
{
    public override ObjectTypeSymbol ContainingSymbol { get; }
    public GenericParameterType DeclaresType { get; }

    public GenericParameterTypeSymbol(
        ObjectTypeSymbol containingSymbol,
        GenericParameterType declaresType)
        : base(containingSymbol, declaresType.Name)
    {
        ContainingSymbol = containingSymbol;
        DeclaresType = declaresType;
    }

    #region Equals
    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is GenericParameterTypeSymbol otherType
               && ContainingSymbol == otherType.ContainingSymbol
               && Name == otherType.Name;
    }

    public override int GetHashCode() => HashCode.Combine(ContainingSymbol, Name);
    #endregion

    public override string ToILString() => $"{ContainingSymbol.ToILString()}.{Name}";
}