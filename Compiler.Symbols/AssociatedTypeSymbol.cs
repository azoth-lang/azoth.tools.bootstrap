using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class AssociatedTypeSymbol : TypeSymbol
{
    public override PackageFacetSymbol? Facet => ContainingSymbol.Facet;
    public override TypeSymbol ContainingSymbol { get; }
    public override TypeSymbol ContextTypeSymbol => ContainingSymbol;
    public AssociatedTypeConstructor TypeConstructor { get; }

    public AssociatedTypeSymbol(TypeSymbol containingSymbol, AssociatedTypeConstructor typeConstructor)
        : base(typeConstructor.Name)
    {
        ContainingSymbol = containingSymbol;
        TypeConstructor = typeConstructor;
    }

    #region Equals
    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is AssociatedTypeSymbol otherType
               && ContainingSymbol == otherType.ContainingSymbol
               && TypeConstructor.Equals(otherType.TypeConstructor);
    }

    public override int GetHashCode() => HashCode.Combine(ContainingSymbol, TypeConstructor);
    #endregion

    public override string ToILString()
    {
        var containSymbolString = ContainingSymbol.ToILString();
        return $"{containSymbolString}.{Name}";
    }
}
