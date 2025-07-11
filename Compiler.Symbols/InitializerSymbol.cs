using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

/// <remarks>Named initializers show up as invocable symbols inside of their containing type.</remarks>
public sealed class InitializerSymbol : FunctionOrInitializerSymbol
{
    public override OrdinaryTypeSymbol ContextTypeSymbol { get; }
    public override OrdinaryTypeSymbol ContainingSymbol { get; }
    public override IdentifierName? Name { get; }
    public CapabilityType SelfParameterType { get; }
    public override NonVoidType ReturnType { get; }

    /// <summary>
    /// The <see cref="FunctionType"/> that will result from passing this initializer as a reference
    /// to something expecting a function type.
    /// </summary>
    public FunctionType InitializerReferenceType { get; }

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
        InitializerReferenceType = new FunctionType(parameterTypes, ReturnType);
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
            && SelfParameterType.Equals(otherInitializer.SelfParameterType)
            && ParameterTypes.SequenceEqual(otherInitializer.ParameterTypes);
    }

    public override int GetHashCode()
        => HashCode.Combine(ContextTypeSymbol, Name, SelfParameterType, ParameterTypes);

    public override string ToILString()
    {
        var name = Name is not null ? $".{Name}" : "";
        var selfParameterType = new ParameterType(false, SelfParameterType);
        return $"{ContextTypeSymbol}::init{name}({string.Join(", ", ParameterTypes.Prepend(selfParameterType).Select(d => d.ToILString()))})";
    }
}
