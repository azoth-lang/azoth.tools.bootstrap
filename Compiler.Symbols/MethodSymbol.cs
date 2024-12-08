using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Parameters;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class MethodSymbol : InvocableSymbol
{
    public override TypeSymbol ContainingSymbol { get; }
    public override TypeSymbol ContextTypeSymbol => ContainingSymbol;
    public MethodKind Kind { get; }
    public override IdentifierName Name { get; }
    public SelfParameterType SelfParameterType { get; }
    public override IType ReturnType { get; }
    public FunctionType MethodGroupType { get; }

    public MethodSymbol(
        TypeSymbol containingSymbol,
        MethodKind kind,
        IdentifierName name,
        SelfParameterType selfParameterType,
        IFixedList<ParameterType> parameterTypes,
        IType returnType)
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
               && SelfParameterType == otherMethod.SelfParameterType
               && ParameterTypes.SequenceEqual(otherMethod.ParameterTypes)
               && ReturnType.Equals(otherMethod.ReturnType);
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
