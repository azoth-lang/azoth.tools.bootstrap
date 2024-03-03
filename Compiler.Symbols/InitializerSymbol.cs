using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;
using ValueType = Azoth.Tools.Bootstrap.Compiler.Types.ValueType;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class InitializerSymbol : InvocableSymbol
{
    // TODO fix types
    public override UserTypeSymbol ContainingSymbol { get; }
    public ValueType SelfParameterType { get; }
    public ValueType ReturnType { get; }

    public InitializerSymbol(
        UserTypeSymbol containingSymbol,
        SimpleName? name,
        ValueType selfParameterType,
        IFixedList<Parameter> parameterTypes)
        : base(containingSymbol, name, parameterTypes,
            new Return(((StructType)containingSymbol.DeclaresType).ToInitializerReturn(selfParameterType, parameterTypes)))
    {
        ContainingSymbol = containingSymbol;
        SelfParameterType = selfParameterType;
        ReturnType = (ValueType)base.Return.Type;
    }

    public static InitializerSymbol CreateDefault(UserTypeSymbol containingSymbol)
        => new(containingSymbol, null, ((StructType)containingSymbol.DeclaresType).ToDefaultInitializerSelf(),
            FixedList.Empty<Parameter>());

    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ConstructorSymbol otherConstructor
            && ContainingSymbol == otherConstructor.ContainingSymbol
            && Name == otherConstructor.Name
            && Parameters.SequenceEqual(otherConstructor.Parameters);
    }

    public override int GetHashCode() => HashCode.Combine(ContainingSymbol, Name, Parameters);

    public override string ToILString()
    {
        var name = Name is null ? $" {Name}" : "";
        var selfParameterType = new Parameter(false, SelfParameterType);
        return
            $"{ContainingSymbol}::new{name}({string.Join(", ", Parameters.Prepend(selfParameterType).Select(d => d.ToILString()))})";
    }
}
