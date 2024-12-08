using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Parameters;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

/// <remarks>Named initializers show up as invocable symbols inside of their containing type.</remarks>
public sealed class InitializerSymbol : FunctionOrInitializerSymbol
{
    public override OrdinaryTypeSymbol ContextTypeSymbol { get; }
    public override OrdinaryTypeSymbol ContainingSymbol { get; }
    public override IdentifierName? Name { get; }
    public CapabilityType SelfParameterType { get; }
    public override CapabilityType ReturnType { get; }
    public FunctionType InitializerGroupType { get; }

    public InitializerSymbol(
        OrdinaryTypeSymbol containingTypeSymbol,
        IdentifierName? initializerName,
        CapabilityType selfParameterType,
        IFixedList<ParameterType> parameterTypes)
        : base(parameterTypes)
    {
        ContextTypeSymbol = containingTypeSymbol;
        ContainingSymbol = containingTypeSymbol;
        Name = initializerName;
        SelfParameterType = selfParameterType;
        ReturnType = containingTypeSymbol.TypeConstructor.ToConstructorReturn(selfParameterType, parameterTypes);
        InitializerGroupType = new FunctionType(parameterTypes, ReturnType);
    }

    public static InitializerSymbol CreateDefault(OrdinaryTypeSymbol containingTypeSymbol)
        => new(containingTypeSymbol, null,
            containingTypeSymbol.TypeConstructor.ToDefaultConstructorSelf(),
            FixedList.Empty<ParameterType>());

    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is InitializerSymbol otherInitializer
            && ContextTypeSymbol == otherInitializer.ContextTypeSymbol
            && Name == otherInitializer.Name
            && ParameterTypes.SequenceEqual(otherInitializer.ParameterTypes);
    }

    public override int GetHashCode()
        => HashCode.Combine(ContextTypeSymbol, Name, ParameterTypes);

    public override string ToILString()
    {
        var name = Name is not null ? $".{Name}" : "";
        var selfParameterType = new ParameterType(false, SelfParameterType);
        return $"{ContextTypeSymbol}::init{name}({string.Join(", ", ParameterTypes.Prepend(selfParameterType).Select(d => d.ToILString()))})";
    }
}
