using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

/// <summary>
/// A symbol for a type declaration (i.e. a class, struct, or trait)
/// </summary>
public sealed class OrdinaryTypeSymbol : TypeSymbol
{
    public override PackageSymbol Package { get; }
    public override Symbol ContainingSymbol { get; }
    public override TypeSymbol? ContextTypeSymbol => null;
    public TypeKind Kind => TypeConstructor.Kind;
    public override OrdinaryName Name { get; }
    public OrdinaryTypeConstructor TypeConstructor { get; }

    public OrdinaryTypeSymbol(
        Symbol containingSymbol,
        OrdinaryTypeConstructor typeConstructor)
        : base(typeConstructor.Name)
    {
        // TODO check the declared type is in the containing namespace and package
        Package = containingSymbol.Package ?? throw new ArgumentException("Must be a proper container for a type.", nameof(containingSymbol));
        ContainingSymbol = containingSymbol;
        Name = typeConstructor.Name;
        TypeConstructor = typeConstructor;
    }

    public override OrdinaryTypeConstructor TryGetTypeConstructor() => TypeConstructor;

    #region Equals
    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is OrdinaryTypeSymbol otherType
               && ContainingSymbol == otherType.ContainingSymbol
               && Name == otherType.Name
               && TypeConstructor.Equals(otherType.TypeConstructor); // Must use Equals because they are interface types
    }

    public override int GetHashCode()
        => HashCode.Combine(ContainingSymbol, Name, TypeConstructor);
    #endregion

    public override string ToILString()
    {
        var genericParametersCount = TypeConstructor.Parameters.Count;
        var value = $"{ContainingSymbol.ToILString()}.{Name.ToBareString()}";
        if (genericParametersCount > 0)
            value += $"[{new string(',', genericParametersCount - 1)}]";
        return value;
    }
}
