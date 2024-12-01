using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class AssociatedTypeSymbol : TypeSymbol
{
    public override PackageSymbol Package => ContainingSymbol.Package ?? throw new ArgumentNullException();
    public override OrdinaryTypeSymbol ContainingSymbol { get; }
    public override OrdinaryTypeSymbol ContextTypeSymbol => ContainingSymbol;
    public BareTypeVariableType Type { get; }

    public AssociatedTypeSymbol(OrdinaryTypeSymbol containingSymbol, BareTypeVariableType type)
        : base(type.Name)
    {
        ContainingSymbol = containingSymbol;
        Type = type;
    }

    #region Equals
    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is AssociatedTypeSymbol otherType
               && ContainingSymbol == otherType.ContainingSymbol
               && Type.Equals(otherType.Type);
    }

    public override int GetHashCode() => HashCode.Combine(ContainingSymbol, Name);
    #endregion

    public override string ToILString()
    {
        var containSymbolString = ContainingSymbol.ToILString();
        return $"{containSymbolString}.{Name}";
    }
}
