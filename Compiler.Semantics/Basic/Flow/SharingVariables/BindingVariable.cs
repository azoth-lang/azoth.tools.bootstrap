using System;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;

public sealed class BindingVariable : ICapabilitySharingVariable
{
    public BindingSymbol Symbol { get; }
    public bool IsVariableOrParameter { get; }
    public CapabilityRestrictions RestrictionsImposed => CapabilityRestrictions.None;
    public Pseudotype Type => Symbol.Type;
    public bool IsLent => Symbol.IsLentBinding;
    public bool SharingIsTracked { get; }
    public bool KeepsSetAlive => true;

    public BindingVariable(BindingSymbol symbol)
    {
        Symbol = symbol;
        IsVariableOrParameter = symbol is not FieldSymbol;
        SharingIsTracked = symbol.SharingIsTracked();
    }

    #region  Equality
    public bool Equals(ISharingVariable? other)
        // All other properties are derived from the symbol
        => other is BindingVariable bindingVariable && Symbol.Equals(bindingVariable.Symbol);

    public override bool Equals(object? obj)
        => obj is BindingVariable other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Symbol);
    #endregion

    public static implicit operator BindingVariable(BindingSymbol symbol)
        => new BindingVariable(symbol);

    public override string ToString() => Symbol.ToILString();
}
