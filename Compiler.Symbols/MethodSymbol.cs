using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class MethodSymbol : InvocableSymbol
{
    public override TypeSymbol ContainingSymbol { get; }
    public override TypeSymbol ContextTypeSymbol => ContainingSymbol;
    public override SimpleName Name { get; }
    public SelfParameter SelfParameterType { get; }

    public MethodSymbol(
        TypeSymbol containingSymbol,
        SimpleName name,
        SelfParameter selfParameterType,
        IFixedList<Parameter> parameters,
        Return @return)
        : base(parameters, @return)
    {
        ContainingSymbol = containingSymbol;
        Name = name;
        SelfParameterType = selfParameterType;
    }

    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is MethodSymbol otherMethod
               && ContainingSymbol == otherMethod.ContainingSymbol
               && Name == otherMethod.Name
               && SelfParameterType == otherMethod.SelfParameterType
               && Parameters.SequenceEqual(otherMethod.Parameters)
               && Return == otherMethod.Return;
    }

    public override int GetHashCode()
        => HashCode.Combine(Name, SelfParameterType, Parameters, Return);

    public override string ToILString()
    {
        var parameterSeparator = Parameters.Any() ? ", " : "";
        string parameters = string.Join(", ", Parameters.Select(d => d.ToILString()));
        return $"{ContainingSymbol.ToILString()}::{Name}({SelfParameterType.ToILString()}{parameterSeparator}{parameters}) -> {Return.ToILString()}";
    }
}
