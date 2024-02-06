using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class SelfParameterSymbol : BindingSymbol
{
    public override InvocableSymbol ContainingSymbol { get; }

    public override Pseudotype Type { get; }

    public SelfParameterSymbol(InvocableSymbol containingSymbol, bool isLent, Pseudotype type)
        : base(containingSymbol, false, isLent, null)
    {
        if (containingSymbol is FunctionSymbol)
            throw new ArgumentException("Function can't have self parameter", nameof(containingSymbol));

        ContainingSymbol = containingSymbol;
        Type = type;
    }

    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is SelfParameterSymbol otherVariable
               && ContainingSymbol == otherVariable.ContainingSymbol
               && Name == otherVariable.Name
               && IsMutableBinding == otherVariable.IsMutableBinding
               && Type == otherVariable.Type;
    }

    public override int GetHashCode()
        => HashCode.Combine(Name, IsMutableBinding, Type);

    public override string ToILString()
    {
        var lent = IsLentBinding ? "lent " : "";
        return $"{ContainingSymbol} {{{lent}self: {Type.ToILString()}}}";
    }
}
