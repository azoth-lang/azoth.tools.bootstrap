using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;
using ValueType = Azoth.Tools.Bootstrap.Compiler.Types.ValueType;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

/// <remarks>Named initializers show up as invocable symbols inside of their containing type.
/// Unnamed initializers show up as invocable symbols alongside their contain type.</remarks>
public sealed class InitializerSymbol : FunctionOrInitializerSymbol
{
    public UserTypeSymbol ContainingTypeSymbol { get; }
    public override Symbol ContainingSymbol { get; }
    public SimpleName? InitializerName { get; }
    public ValueType SelfParameterType { get; }
    public ValueType ReturnType { get; }

    public InitializerSymbol(
        UserTypeSymbol containingTypeSymbol,
        SimpleName? initializerName,
        ValueType selfParameterType,
        IFixedList<Parameter> parameterTypes)
        : base(initializerName ?? containingTypeSymbol.Name.Text, parameterTypes,
            new Return(((StructType)containingTypeSymbol.DeclaresType).ToInitializerReturn(selfParameterType, parameterTypes)))
    {
        bool isNamed = initializerName is not null;
        ContainingTypeSymbol = containingTypeSymbol;
        ContainingSymbol = isNamed ? containingTypeSymbol : containingTypeSymbol.ContainingSymbol;
        InitializerName = initializerName;
        SelfParameterType = selfParameterType;
        ReturnType = (ValueType)base.Return.Type;
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
            && ContainingTypeSymbol == otherInitializer.ContainingTypeSymbol
            && InitializerName == otherInitializer.InitializerName
            && Parameters.SequenceEqual(otherInitializer.Parameters);
    }

    public override int GetHashCode() => HashCode.Combine(ContainingTypeSymbol, InitializerName, Parameters);

    public override string ToILString()
    {
        var name = InitializerName is null ? $".{InitializerName}" : "";
        var selfParameterType = new Parameter(false, SelfParameterType);
        return $"{ContainingTypeSymbol}::init{name}({string.Join(", ", Parameters.Prepend(selfParameterType).Select(d => d.ToILString()))})";
    }
}
