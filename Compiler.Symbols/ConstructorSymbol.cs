using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class ConstructorSymbol : InvocableSymbol
{
    public override ObjectTypeSymbol ContainingSymbol { get; }
    public ObjectType SelfParameterType { get; }
    public override ObjectType ReturnDataType { get; }

    public ConstructorSymbol(
        ObjectTypeSymbol containingSymbol,
        Name? name,
        ObjectType selfParameterType,
        FixedList<DataType> parameterDataTypes)
        : base(containingSymbol, name, parameterDataTypes,
            containingSymbol.DeclaresType.ToDefaultConstructorReturn())
    {
        ContainingSymbol = containingSymbol;
        SelfParameterType = selfParameterType;
        ReturnDataType = containingSymbol.DeclaresType.ToConstructorReturn(selfParameterType, parameterDataTypes);
    }

    public static ConstructorSymbol CreateDefault(ObjectTypeSymbol containingSymbol)
        => new(containingSymbol, null, containingSymbol.DeclaresType.ToDefaultConstructorSelf(), FixedList<DataType>.Empty);

    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ConstructorSymbol otherConstructor
               && ContainingSymbol == otherConstructor.ContainingSymbol
               && Name == otherConstructor.Name
               && ParameterDataTypes.SequenceEqual(otherConstructor.ParameterDataTypes);
    }

    public override int GetHashCode()
        => HashCode.Combine(ContainingSymbol, Name, ParameterDataTypes);

    public override string ToILString()
    {
        var name = Name is null ? $" {Name}" : "";
        return $"{ContainingSymbol}::new{name}({string.Join(", ", ParameterDataTypes.Select(d => d.ToILString()))})";
    }
}
