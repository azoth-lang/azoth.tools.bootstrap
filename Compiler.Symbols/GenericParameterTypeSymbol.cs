using System;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class GenericParameterTypeSymbol : TypeSymbol
{
    public override PackageSymbol Package => ContainingSymbol.Package ?? throw new ArgumentNullException();
    public override UserTypeSymbol ContainingSymbol { get; }
    public override UserTypeSymbol ContextTypeSymbol => ContainingSymbol;
    public GenericParameterType DeclaresType { get; }

    public GenericParameterTypeSymbol(
        UserTypeSymbol containingSymbol,
        GenericParameterType declaresType)
        : base(declaresType.Name)
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

    public override string ToILString()
    {
        var containSymbolString = ContainingSymbol.ToILString();
        return $"{containSymbolString}.{Name}";
    }
}
