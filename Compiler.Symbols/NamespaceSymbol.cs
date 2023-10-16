using System;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

/// <summary>
/// While namespaces in syntax declarations are repeated across files, and
/// IL doesn't even directly represent namespaces, for symbols, a namespace
/// is the container of all the names in it. There is one symbol per namespace.
/// </summary>
public sealed class NamespaceSymbol : NamespaceOrPackageSymbol
{
    public override NamespaceOrPackageSymbol ContainingSymbol { get; }

    public NamespaceSymbol(NamespaceOrPackageSymbol containingSymbol, SimpleName name)
        : base(containingSymbol, name)
    {
        ContainingSymbol = containingSymbol;
    }

    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is NamespaceSymbol otherNamespace
               && ContainingSymbol.Equals(otherNamespace.ContainingSymbol)
               && Name == otherNamespace.Name;
    }

    public override int GetHashCode() => HashCode.Combine(ContainingSymbol, Name);

    public override string ToILString() => $"{ContainingSymbol.ToILString()}.{Name}";
}
