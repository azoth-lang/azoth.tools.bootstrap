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
    public virtual PackageSymbol? Package { get; }
    public Symbol? ContainingSymbol { get; }
    public TypeName? Name { get; }
    public bool IsGlobal => ContainingSymbol == Package;

    protected Symbol(Symbol? containingSymbol, TypeName? name)
    {
        // Note: constructor can't be `private protected` so `Symbol` can be mocked in unit tests
        Package = containingSymbol?.Package;
        ContainingSymbol = containingSymbol;
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
        => !(symbol1 == symbol2);
    #endregion

    [Obsolete("Use ToILString() instead", error: true)]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    public sealed override string ToString()
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
        => ToILString();

    public abstract string ToILString();
}
