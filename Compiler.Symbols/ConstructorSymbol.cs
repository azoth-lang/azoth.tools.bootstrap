using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class ConstructorSymbol : InvocableSymbol
{
    public override UserTypeSymbol ContainingSymbol { get; }
    public override UserTypeSymbol ContextTypeSymbol => ContainingSymbol;
    public override IdentifierName? Name { get; }
    public CapabilityType SelfParameterType { get; }
    public CapabilityType ReturnType { get; }

    public ConstructorSymbol(
        UserTypeSymbol containingSymbol,
        IdentifierName? name,
        CapabilityType selfParameterType,
        IFixedList<ParameterType> parameterTypes)
        : base(parameterTypes,
            ((ObjectType)containingSymbol.DeclaresType).ToConstructorReturn(selfParameterType, parameterTypes))
    {
        ContainingSymbol = containingSymbol;
        Name = name;
        SelfParameterType = selfParameterType;
        ReturnType = (CapabilityType)Return;
    }

    public static ConstructorSymbol CreateDefault(UserTypeSymbol containingSymbol)
        => new(containingSymbol, null,
            ((ObjectType)containingSymbol.DeclaresType).ToDefaultConstructorSelf(),
            FixedList.Empty<ParameterType>());

    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ConstructorSymbol otherConstructor
               && ContainingSymbol == otherConstructor.ContainingSymbol
               && Name == otherConstructor.Name
               && Parameters.SequenceEqual(otherConstructor.Parameters);
    }

    public override int GetHashCode()
        => HashCode.Combine(ContainingSymbol, Name, Parameters);

    public override string ToILString()
    {
        var name = Name is null ? $".{Name}" : "";
        var selfParameterType = new ParameterType(false, SelfParameterType);
        return $"{ContainingSymbol}::new{name}({string.Join(", ", Parameters.Prepend(selfParameterType).Select(d => d.ToILString()))})";
    }
}
