using System;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

/// <summary>
/// A symbol for a non-global namespace.
/// </summary>
/// <remarks><para>There is no separate symbol for the global namespace. The package symbol is used
/// directly when referring to the global namespace.</para>
/// <para>
/// While namespaces in syntax declarations are repeated across files, and
/// IL doesn't even directly represent namespaces, for symbols, a namespace
/// is the container of all the names in it. There is one symbol per namespace.</para>
/// </remarks>
public sealed class LocalNamespaceSymbol : NamespaceSymbol
{
    public override NamespaceSymbol ContainingSymbol { get; }
    public override TypeSymbol? ContextTypeSymbol => null;

    public LocalNamespaceSymbol(NamespaceSymbol containingSymbol, IdentifierName name)
        : base(containingSymbol.Package, containingSymbol, name)
    {
        ContainingSymbol = containingSymbol;
    }

    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is LocalNamespaceSymbol otherNamespace
               && ContainingSymbol.Equals(otherNamespace.ContainingSymbol)
               && Name == otherNamespace.Name;
    }

    public override int GetHashCode() => HashCode.Combine(ContainingSymbol, Name);

    public override string ToILString() => $"{ContainingSymbol.ToILString()}.{Name}";
}
