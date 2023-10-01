using System;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

/// <summary>
/// A variable that can participate in sharing. This is all the <see cref="BindingSymbol"/>s
/// plus special values representing the result of an expression.
/// </summary>
public readonly struct SharingVariable : IEquatable<SharingVariable>
{
    private readonly long result;

    public SharingVariable(BindingSymbol symbol)
    {
        Symbol = symbol;
        result = default;
    }

    public SharingVariable(long resultNumber)
    {
        Symbol = null;
        result = resultNumber;
    }

    public bool IsLocal => Symbol is VariableSymbol { IsLocal: true };
    public bool IsResult => SymbolType is null;
    public DataType? SymbolType => Symbol?.DataType;
    public BindingSymbol? Symbol { get; }

    #region Equals
    public bool Equals(SharingVariable other)
        => Equals(Symbol, other.Symbol) && result == other.result;

    public override bool Equals(object? obj)
        => obj is SharingVariable other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Symbol, result);

    public static bool operator ==(SharingVariable left, SharingVariable right)
        => left.Equals(right);

    public static bool operator !=(SharingVariable left, SharingVariable right)
        => !left.Equals(right);
    #endregion

    public static implicit operator SharingVariable(BindingSymbol symbol)
        => new(symbol);

    public static implicit operator SharingVariable?(BindingSymbol? symbol)
        => symbol is null ? null : new(symbol);

    public override string ToString() => Symbol?.ToString() ?? $"⧼result{result}⧽";
}
