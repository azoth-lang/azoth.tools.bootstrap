using System;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class GenericParameterTypeSymbol : TypeSymbol
{
    private readonly IPromise<UserTypeSymbol> containingSymbol;
    public override PackageSymbol Package => ContainingSymbol.Package ?? throw new ArgumentNullException();
    public override UserTypeSymbol ContainingSymbol => containingSymbol.Result;
    public GenericParameterType DeclaresType { get; }

    public GenericParameterTypeSymbol(
        IPromise<UserTypeSymbol> containingSymbol,
        GenericParameterType declaresType)
        : base(declaresType.Name)
    {
        this.containingSymbol = containingSymbol;
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
        var containSymbolString = containingSymbol.IsFulfilled ? ContainingSymbol.ToILString() : containingSymbol.ToString();
        return $"{containSymbolString}.{Name}";
    }
}
