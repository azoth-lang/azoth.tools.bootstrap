using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Framework;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class ConstructorSymbol : InvocableSymbol
{
    public override OrdinaryTypeSymbol ContainingSymbol { get; }
    public override OrdinaryTypeSymbol ContextTypeSymbol => ContainingSymbol;
    public override IdentifierName? Name { get; }
    public CapabilityType SelfParameterType { get; }
    public override Type ReturnType { get; }

    public ConstructorSymbol(
        OrdinaryTypeSymbol containingSymbol,
        IdentifierName? name,
        CapabilityType selfParameterType,
        IFixedList<ParameterType> parameterTypes)
        : base(parameterTypes)
    {
        ContainingSymbol = containingSymbol;
        Name = name;
        SelfParameterType = selfParameterType;
        ReturnType = containingSymbol.TypeConstructor.ToConstructorReturn(selfParameterType, parameterTypes);
    }

    public static ConstructorSymbol CreateDefault(OrdinaryTypeSymbol containingSymbol)
        => new(containingSymbol, null,
            containingSymbol.TypeConstructor.ToDefaultConstructorSelf(),
            FixedList.Empty<ParameterType>());

    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ConstructorSymbol otherConstructor
               && ContainingSymbol == otherConstructor.ContainingSymbol
               && Name == otherConstructor.Name
               && SelfParameterType.Equals(otherConstructor.SelfParameterType)
               && ParameterTypes.SequenceEqual(otherConstructor.ParameterTypes);
    }

    public override int GetHashCode()
        => HashCode.Combine(ContainingSymbol, Name, SelfParameterType, ParameterTypes);

    public override string ToILString()
    {
        var name = Name is null ? $".{Name}" : "";
        var selfParameterType = new ParameterType(false, SelfParameterType);
        return $"{ContainingSymbol}::new{name}({string.Join(", ", ParameterTypes.Prepend(selfParameterType).Select(d => d.ToILString()))})";
    }
}
