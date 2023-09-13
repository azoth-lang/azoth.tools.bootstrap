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
    private readonly BindingSymbol? symbol;
    private readonly long result;

    public SharingVariable(BindingSymbol symbol)
    {
        this.symbol = symbol;
        result = default;
    }

    public SharingVariable(long resultNumber)
    {
        symbol = null;
        result = resultNumber;
    }

    public bool IsLocal => symbol is VariableSymbol { IsLocal: true };
    public DataType? SymbolType => symbol?.DataType;

    #region Equals
    public bool Equals(SharingVariable other)
        => Equals(symbol, other.symbol) && result == other.result;

    public override bool Equals(object? obj)
        => obj is SharingVariable other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(symbol, result);

    public static bool operator ==(SharingVariable left, SharingVariable right)
        => left.Equals(right);

    public static bool operator !=(SharingVariable left, SharingVariable right)
        => !left.Equals(right);
    #endregion

    public static implicit operator SharingVariable(BindingSymbol symbol)
        => new(symbol);

    public static implicit operator SharingVariable?(BindingSymbol? symbol)
        => symbol is null ? null : new(symbol);

    public override string ToString() => symbol?.ToString() ?? $"⧼result{result}⧽";
}
