using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class GenericParameterTypeSymbol : TypeSymbol
{
    public override PackageFacetSymbol Facet => ContainingSymbol.Facet;
    public override OrdinaryTypeSymbol ContainingSymbol { get; }
    public override OrdinaryTypeSymbol ContextTypeSymbol => ContainingSymbol;
    public GenericParameterPlainType PlainType { get; }
    public override IdentifierName Name => (IdentifierName)base.Name;

    public override PlainType TryGetPlainType() => PlainType;

    public GenericParameterTypeSymbol(
        OrdinaryTypeSymbol containingSymbol,
        GenericParameterPlainType plainType)
        : base(plainType.Name)
    {
        ContainingSymbol = containingSymbol;
        PlainType = plainType;
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

    public override string ToILString()
    {
        var containSymbolString = ContainingSymbol.ToILString();
        return $"{containSymbolString}.{Name}";
    }
}
