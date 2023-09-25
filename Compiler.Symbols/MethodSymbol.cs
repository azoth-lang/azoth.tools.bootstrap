using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class MethodSymbol : FunctionOrMethodSymbol
{
    public override TypeSymbol ContainingSymbol { get; }
    public override Name Name { get; }
    public DataType SelfDataType { get; }

    public MethodSymbol(
        TypeSymbol containingSymbol,
        Name name,
        DataType selfDataType,
        FixedList<DataType> parameterDataTypes,
        DataType returnDataType)
        : base(containingSymbol, name, parameterDataTypes, returnDataType)
    {
        ContainingSymbol = containingSymbol;
        Name = name;
        SelfDataType = selfDataType;
    }

    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is MethodSymbol otherMethod
               && ContainingSymbol == otherMethod.ContainingSymbol
               && Name == otherMethod.Name
               && SelfDataType == otherMethod.SelfDataType
               && ParameterDataTypes.SequenceEqual(otherMethod.ParameterDataTypes)
               && ReturnDataType == otherMethod.ReturnDataType;
    }

    public override int GetHashCode()
        => HashCode.Combine(Name, SelfDataType, ParameterDataTypes, ReturnDataType);

    public override string ToILString()
    {
        var selfCapability = SelfDataType is ReferenceType referenceType ? referenceType.Capability.ToILString() + " " : "";
        string parameters = string.Join(", ", ParameterDataTypes.Select(d => d.ToILString()).Prepend($"{selfCapability}self"));
        return $"{ContainingSymbol.ToILString()}::{Name}({parameters}) -> {ReturnDataType.ToILString()}";
    }
}
