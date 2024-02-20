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
        : base(containingSymbol, name, type.Parameters, type.Return)
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
               && Parameters.SequenceEqual(otherFunction.Parameters)
               && Return == otherFunction.Return;
    }

    public override int GetHashCode()
        => HashCode.Combine(ContainingSymbol, Name, Parameters, Return);

    public override string ToILString()
        => $"{ContainingSymbol.ToILString()}.{Name}({string.Join(", ", Parameters.Select(d => d.ToILString()))}) -> {Return.ToILString()}";
}
