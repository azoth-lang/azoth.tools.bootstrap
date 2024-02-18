using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(
    typeof(NamespaceOrPackageSymbol),
    typeof(TypeSymbol),
    typeof(InvocableSymbol),
    typeof(BindingSymbol))]
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public abstract class Symbol : IEquatable<Symbol>
{
    /// <summary>
    /// The package that declares this symbol or <see langword="null"/> for primitives.
    /// </summary>
    public abstract PackageSymbol? Package { get; }
    /// <summary>
    /// The symbol that contains this symbol or <see langword="null"/> for primitives and
    /// <see cref="PackageSymbol"/>.
    /// </summary>
    public abstract Symbol? ContainingSymbol { get; }
    public virtual TypeName? Name { get; }
    public bool IsGlobal => ContainingSymbol == Package;

    private protected Symbol(TypeName? name)
    {
        Name = name;
    }

    #region Equality
    public abstract bool Equals(Symbol? other);

    public abstract override int GetHashCode();

    public sealed override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Symbol)obj);
    }

    public static bool operator ==(Symbol? symbol1, Symbol? symbol2)
        => Equals(symbol1, symbol2);

    public static bool operator !=(Symbol? symbol1, Symbol? symbol2)
        => !Equals(symbol1, symbol2);
    #endregion

    [Obsolete("Use ToILString() instead", error: true)]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    public sealed override string ToString()
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
        => ToILString();

    public abstract string ToILString();
}
