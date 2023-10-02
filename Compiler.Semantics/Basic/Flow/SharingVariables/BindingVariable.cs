using System;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;

public sealed class BindingVariable : ISharingVariable
{
    public BindingSymbol Symbol { get; }
    public bool IsVariableOrParameter { get; }
    public bool RestrictsWrite => false;
    public DataType DataType => Symbol.DataType;
    public bool IsLent => Symbol.IsLentBinding;

    public BindingVariable(BindingSymbol symbol)
    {
        Symbol = symbol;
        IsVariableOrParameter = symbol is not FieldSymbol;
    }

    #region  Equality
    public bool Equals(ISharingVariable? other)
        => other is BindingVariable bindingVariable && Symbol.Equals(bindingVariable.Symbol);

    public override bool Equals(object? obj)
        => obj is BindingVariable other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Symbol);
    #endregion

    public static implicit operator BindingVariable(BindingSymbol symbol)
        => new BindingVariable(symbol);

    public override string ToString() => Symbol.ToILString();
}
