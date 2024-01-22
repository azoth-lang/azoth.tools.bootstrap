using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

/// <summary>
/// A symbol for a function
/// </summary>
public sealed class FunctionSymbol : FunctionOrMethodSymbol
{
    public override SimpleName Name { get; }
    public FunctionType Type { get; }

    public FunctionSymbol(
        Symbol containingSymbol,
        SimpleName name,
        FunctionType type)
        : base(containingSymbol, name, type.ParameterTypes, type.ReturnType)
    {
        Name = name;
        Type = type;
    }

    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is FunctionSymbol otherFunction
               && ContainingSymbol == otherFunction.ContainingSymbol
               && Name == otherFunction.Name
               && ParameterTypes.SequenceEqual(otherFunction.ParameterTypes)
               && ReturnType == otherFunction.ReturnType;
    }

    public override int GetHashCode()
        => HashCode.Combine(ContainingSymbol, Name, ParameterTypes, ReturnType);

    public override string ToILString()
        => $"{ContainingSymbol.ToILString()}.{Name}({string.Join(", ", ParameterTypes.Select(d => d.ToILString()))}) -> {ReturnType.ToILString()}";
}
