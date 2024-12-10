using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

/// <summary>
/// A symbol for a standalone or associated function.
/// </summary>
public sealed class FunctionSymbol : FunctionOrInitializerSymbol
{
    public override Symbol ContainingSymbol { get; }
    public override TypeSymbol? ContextTypeSymbol => null;
    public override IdentifierName Name { get; }
    public override IType ReturnType { get; }
    public FunctionType Type { get; }

    public FunctionSymbol(
        Symbol containingSymbol,
        IdentifierName name,
        FunctionType type)
        : base(type.Parameters)
    {
        ContainingSymbol = containingSymbol;
        Name = name;
        Type = type;
        ReturnType = type.Return;
    }

    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is FunctionSymbol otherFunction
               && ContainingSymbol == otherFunction.ContainingSymbol
               && Name == otherFunction.Name
               && ParameterTypes.SequenceEqual(otherFunction.ParameterTypes)
               && ReturnType.Equals(otherFunction.ReturnType);
    }

    public override int GetHashCode()
        => HashCode.Combine(ContainingSymbol, Name, ParameterTypes, ReturnType);

    public override string ToILString()
        => $"{ContainingSymbol.ToILString()}.{Name}({string.Join(", ", ParameterTypes.Select(d => d.ToILString()))}) -> {ReturnType.ToILString()}";
}
