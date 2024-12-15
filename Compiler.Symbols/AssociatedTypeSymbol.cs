using System;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class AssociatedTypeSymbol : TypeSymbol
{
    public override PackageSymbol Package => ContainingSymbol.Package;
    public override OrdinaryTypeSymbol ContainingSymbol { get; }
    public override OrdinaryTypeSymbol ContextTypeSymbol => ContainingSymbol;
    public AssociatedTypeFactory TypeFactory { get; }
    public AssociatedPlainType PlainType => TypeFactory.PlainType;

    public AssociatedTypeSymbol(OrdinaryTypeSymbol containingSymbol, AssociatedTypeFactory typeFactory)
        : base(typeFactory.PlainType.Name)
    {
        ContainingSymbol = containingSymbol;
        TypeFactory = typeFactory;
    }

    #region Equals
    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is AssociatedTypeSymbol otherType
               && ContainingSymbol == otherType.ContainingSymbol
               // TODO use TypeFactory for this?
               && PlainType.Equals(otherType.PlainType);
    }

    public override int GetHashCode() => HashCode.Combine(ContainingSymbol, Name);
    #endregion

    public override string ToILString()
    {
        var containSymbolString = ContainingSymbol.ToILString();
        return $"{containSymbolString}.{Name}";
    }
}
