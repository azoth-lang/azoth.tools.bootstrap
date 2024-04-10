using System;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

/// <summary>
/// A symbol for a package.
/// </summary>
/// <remarks>
/// A package alias has no effect on the symbol. It is still the same package.
/// </remarks>
public class PackageSymbol : NamespaceSymbol
{
    public override PackageSymbol Package => this;
    public override Symbol? ContainingSymbol => null;
    public override TypeSymbol? ContextTypeSymbol => null;

    public PackageSymbol(IdentifierName name)
        : base(null!, null, name) { }

    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is PackageSymbol otherPackageSymbol
               && Name == otherPackageSymbol.Name;
    }

    public override int GetHashCode() => HashCode.Combine(Name);

    public override string ToILString() => $"{Name}::";
}
