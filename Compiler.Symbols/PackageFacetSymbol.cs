using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

/// <summary>
/// The dollar separator is good because it won't be used in Azoth syntax
/// </summary>
public sealed class PackageFacetSymbol : NamespaceSymbol
{
    public override PackageSymbol Package { get; }
    public override PackageFacetSymbol Facet => this;
    public FacetKind Kind { get; }
    public override PackageSymbol ContainingSymbol => Package;
    public override TypeSymbol? ContextTypeSymbol => null;
    public override NamespaceName NamespaceName => NamespaceName.Global;
    public override IdentifierName? Name => null;

    public PackageFacetSymbol(PackageSymbol package, FacetKind kind)
    {
        Package = package;
        Kind = kind;
    }

    #region Equality
    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is PackageFacetSymbol otherFacet
               && Kind == otherFacet.Kind
               && ContainingSymbol.Equals(otherFacet.ContainingSymbol);
    }

    public override int GetHashCode() => HashCode.Combine(Package, Kind);
    #endregion

    public override string ToILString() => $"{Package}${Kind}::";
}
