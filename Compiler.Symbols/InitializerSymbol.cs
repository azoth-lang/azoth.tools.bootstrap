using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;
using ValueType = Azoth.Tools.Bootstrap.Compiler.Types.ValueType;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

/// <remarks>Named initializers show up as invocable symbols inside of their containing type.</remarks>
public sealed class InitializerSymbol : FunctionOrInitializerSymbol
{
    public override UserTypeSymbol ContextTypeSymbol { get; }
    public override UserTypeSymbol ContainingSymbol { get; }
    public override IdentifierName? Name { get; }
    public ValueType SelfParameterType { get; }
    public ValueType ReturnType { get; }
    public FunctionType InitializerGroupType { get; }

    public InitializerSymbol(
        UserTypeSymbol containingTypeSymbol,
        IdentifierName? initializerName,
        ValueType selfParameterType,
        IFixedList<Parameter> parameterTypes)
        : base(parameterTypes,
            new Return(((StructType)containingTypeSymbol.DeclaresType).ToInitializerReturn(selfParameterType, parameterTypes)))
    {
        ContextTypeSymbol = containingTypeSymbol;
        ContainingSymbol = containingTypeSymbol;
        Name = initializerName;
        SelfParameterType = selfParameterType;
        ReturnType = (ValueType)Return.Type;
        InitializerGroupType = new FunctionType(parameterTypes, Return);
    }

    public static InitializerSymbol CreateDefault(UserTypeSymbol containingTypeSymbol)
        => new(containingTypeSymbol, null,
            ((StructType)containingTypeSymbol.DeclaresType).ToDefaultInitializerSelf(),
            FixedList.Empty<Parameter>());

    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is InitializerSymbol otherInitializer
            && ContextTypeSymbol == otherInitializer.ContextTypeSymbol
            && Name == otherInitializer.Name
            && Parameters.SequenceEqual(otherInitializer.Parameters);
    }

    public override int GetHashCode()
        => HashCode.Combine(ContextTypeSymbol, Name, Parameters);

    public override string ToILString()
    {
        var name = Name is not null ? $".{Name}" : "";
        var selfParameterType = new Parameter(false, SelfParameterType);
        return $"{ContextTypeSymbol}::init{name}({string.Join(", ", Parameters.Prepend(selfParameterType).Select(d => d.ToILString()))})";
    }
}
