using System;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

/// <summary>
/// A symbol for a package. This is also used as the symbol for the global namespace.
/// </summary>
/// <remarks>
/// A package alias has no effect on the symbol. It is still the same package.
/// </remarks>
public sealed class PackageSymbol : Symbol
{
    public override PackageSymbol Package => this;
    public override PackageFacetSymbol? Facet => null;
    public override Symbol? ContainingSymbol => null;
    public override TypeSymbol? ContextTypeSymbol => null;
    public override IdentifierName Name { get; }

    public PackageSymbol(IdentifierName name)
    {
        Name = name;
    }

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
