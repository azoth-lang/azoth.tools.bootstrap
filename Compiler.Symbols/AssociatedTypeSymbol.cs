using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class AssociatedTypeSymbol : TypeSymbol
{
    public override PackageSymbol? Package => ContainingSymbol.Package;
    public override TypeSymbol ContainingSymbol { get; }
    public override TypeSymbol ContextTypeSymbol => ContainingSymbol;
    public AssociatedTypeConstructor TypeFactory { get; }
    public ConstructedPlainType PlainType => TypeFactory.PlainType;

    public AssociatedTypeSymbol(TypeSymbol containingSymbol, AssociatedTypeConstructor typeFactory)
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
