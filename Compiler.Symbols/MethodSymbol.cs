using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class MethodSymbol : FunctionOrMethodSymbol
{
    public override TypeSymbol ContainingSymbol { get; }
    public override SimpleName Name { get; }
    public SelfParameterType SelfParameterType { get; }

    public MethodSymbol(
        TypeSymbol containingSymbol,
        SimpleName name,
        SelfParameterType selfParameterType,
        FixedList<ParameterType> parameterTypes,
        ReturnType returnType)
        : base(containingSymbol, name, parameterTypes, returnType)
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
               && ParameterTypes.SequenceEqual(otherMethod.ParameterTypes)
               && ReturnType == otherMethod.ReturnType;
    }

    public override int GetHashCode()
        => HashCode.Combine(Name, SelfParameterType, ParameterTypes, ReturnType);

    public override string ToILString()
    {
        var parameterSeparator = ParameterTypes.Any() ? ", " : "";
        string parameters = string.Join(", ", ParameterTypes.Select(d => d.ToILString()));
        return $"{ContainingSymbol.ToILString()}::{Name}({SelfParameterType.ToILString()}{parameterSeparator}{parameters}) -> {ReturnType.ToILString()}";
    }
}
