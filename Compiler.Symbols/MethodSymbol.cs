using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class MethodSymbol : InvocableSymbol
{
    public override TypeSymbol ContainingSymbol { get; }
    public override TypeSymbol ContextTypeSymbol => ContainingSymbol;
    public MethodKind Kind { get; }
    public override IdentifierName Name { get; }
    public NonVoidType SelfParameterType { get; }
    public override Type ReturnType { get; }
    public FunctionType MethodGroupType { get; }

    public MethodSymbol(
        TypeSymbol containingSymbol,
        MethodKind kind,
        IdentifierName name,
        // TODO once `Self` is used, this can just be a capability constraint
        NonVoidType selfParameterType,
        IFixedList<ParameterType> parameterTypes,
        Type returnType)
        : base(parameterTypes)
    {
        ContainingSymbol = containingSymbol;
        Kind = kind;
        Name = name;
        SelfParameterType = selfParameterType;
        ReturnType = returnType;
        MethodGroupType = new FunctionType(parameterTypes, returnType);
    }

    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is MethodSymbol otherMethod
               && ContainingSymbol == otherMethod.ContainingSymbol
               && Name == otherMethod.Name
               && SelfParameterType.Equals(otherMethod.SelfParameterType)
               && ParameterTypes.SequenceEqual(otherMethod.ParameterTypes)
               && ReturnType.Equals(otherMethod.ReturnType);
    }

    public override int GetHashCode()
        => HashCode.Combine(Name, SelfParameterType, ParameterTypes, ReturnType);

    public override string ToILString()
    {
        var kind = Kind switch
        {
            MethodKind.Getter => ".get",
            MethodKind.Setter => ".set",
            MethodKind.Standard => "",
            _ => throw ExhaustiveMatch.Failed(Kind),
        };
        var parameterSeparator = ParameterTypes.Any() ? ", " : "";
        string parameters = string.Join(", ", ParameterTypes.Select(d => d.ToILString()));
        return $"{ContainingSymbol.ToILString()}::{Name}{kind}({SelfParameterType.ToILString()}{parameterSeparator}{parameters}) -> {ReturnType.ToILString()}";
    }
}
