using System;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class SelfParameterSymbol : BindingSymbol
{
    public override InvocableSymbol ContainingSymbol { get; }

    public SelfParameterSymbol(InvocableSymbol containingSymbol, DataType dataType)
        : base(containingSymbol, null, false, dataType)
    {
        if (containingSymbol is FunctionSymbol)
            throw new ArgumentException("Function can't have self parameter", nameof(containingSymbol));

        ContainingSymbol = containingSymbol;
    }

    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is VariableSymbol otherVariable
               && ContainingSymbol == otherVariable.ContainingSymbol
               && Name == otherVariable.Name
               && IsMutableBinding == otherVariable.IsMutableBinding
               && DataType == otherVariable.DataType;
    }

    public override int GetHashCode()
        => HashCode.Combine(Name, IsMutableBinding, DataType);

    public override string ToILString()
        => $"{ContainingSymbol} {{self: {DataType.ToILString()}}}";
}
