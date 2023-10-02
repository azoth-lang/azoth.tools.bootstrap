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
    private readonly long number;

    public SharingVariable(BindingSymbol symbol)
    {
        Symbol = symbol;
        number = default;
    }

    private SharingVariable(long number)
    {
        Symbol = null;
        this.number = number;
    }

    public bool IsLocal => Symbol is VariableSymbol { IsLocal: true };
    public bool IsParameter => Symbol is VariableSymbol { IsParameter: true } or SelfParameterSymbol;
    public bool IsResult => SymbolType is null && number >= 0;
    public DataType? SymbolType => Symbol?.DataType;
    public BindingSymbol? Symbol { get; }
    public bool IsLent
        => number < ExternalReference.NonParameters.Id
           || Symbol?.DataType is ReferenceType { IsLentReference: true };

    #region Equals
    public bool Equals(SharingVariable other)
        => Equals(Symbol, other.Symbol) && number == other.number;

    public override bool Equals(object? obj)
        => obj is SharingVariable other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Symbol, number);

    public static bool operator ==(SharingVariable left, SharingVariable right)
        => left.Equals(right);

    public static bool operator !=(SharingVariable left, SharingVariable right)
        => !left.Equals(right);
    #endregion

    public static implicit operator SharingVariable(BindingSymbol symbol)
        => new(symbol);

    public static implicit operator SharingVariable?(BindingSymbol? symbol)
        => symbol is null ? null : new(symbol);

    public static implicit operator SharingVariable(ResultVariable variable)
        => new SharingVariable(variable.Number);

    public static implicit operator SharingVariable(ExternalReference reference)
        => new SharingVariable(reference.Id);


    public override string ToString()
        => Symbol?.ToString() ?? (number >= 0 ? $"⧼result{number}⧽" : ExternalReference.ToString(number));
}
