using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

/// <summary>
/// A symbol for a function
/// </summary>
public sealed class FunctionSymbol : FunctionOrMethodSymbol
{
    public override Name Name { get; }

    public FunctionSymbol(
        Symbol containingSymbol,
        Name name,
        FixedList<ParameterType> parameterDataTypes,
        ReturnType returnType)
        : base(containingSymbol, name, parameterDataTypes, returnType)
    {
        Name = name;
    }

    public FunctionSymbol(
        Symbol containingSymbol,
        Name name,
        FixedList<ParameterType> parameterDataTypes)
        : this(containingSymbol, name, parameterDataTypes, ReturnType.Void)
    {
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
