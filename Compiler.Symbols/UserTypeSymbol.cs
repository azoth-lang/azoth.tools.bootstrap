using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

/// <summary>
/// A symbol for a type declaration (i.e. a class, struct, or trait)
/// </summary>
public sealed class UserTypeSymbol : TypeSymbol
{
    public override PackageSymbol Package { get; }
    public override Symbol ContainingSymbol { get; }
    public override TypeSymbol? ContextTypeSymbol => null;
    public override StandardName Name { get; }
    public IDeclaredUserType DeclaresType { get; }

    public UserTypeSymbol(
        Symbol containingSymbol,
        IDeclaredUserType declaresType)
        : base(declaresType.Name)
    {
        // TODO check the declared type is in the containing namespace and package
        Package = containingSymbol.Package ?? throw new ArgumentException("Must be a proper container for a type.", nameof(containingSymbol));
        ContainingSymbol = containingSymbol;
        Name = declaresType.Name;
        DeclaresType = declaresType;
    }

    public override DeclaredType GetDeclaredType() => DeclaresType.AsDeclaredType;

    #region Equals
    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is UserTypeSymbol otherType
               && ContainingSymbol == otherType.ContainingSymbol
               && Name == otherType.Name
               && DeclaresType.Equals(otherType.DeclaresType); // Must use Equals because they are interface types
    }

    public override int GetHashCode()
        => HashCode.Combine(ContainingSymbol, Name, DeclaresType);
    #endregion

    public override string ToILString()
    {
        var genericParametersCount = DeclaresType.GenericParameters.Count;
        var value = $"{ContainingSymbol.ToILString()}.{Name.ToBareString()}";
        if (genericParametersCount > 0)
            value += $"[{new string(',', genericParametersCount - 1)}]";
        return value;
    }
}
