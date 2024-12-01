using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class GenericParameterTypeSymbol : TypeSymbol
{
    public override PackageSymbol Package => ContainingSymbol.Package ?? throw new ArgumentNullException();
    public override OrdinaryTypeSymbol ContainingSymbol { get; }
    public override OrdinaryTypeSymbol ContextTypeSymbol => ContainingSymbol;
    public GenericParameterType Type { get; }
    public override IdentifierName Name => (IdentifierName)base.Name;

    public override IType TryGetType() => Type;

    public GenericParameterTypeSymbol(
        OrdinaryTypeSymbol containingSymbol,
        GenericParameterType type)
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
