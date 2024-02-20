using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class ConstructorSymbol : InvocableSymbol
{
    public override ObjectTypeSymbol ContainingSymbol { get; }
    public ReferenceType SelfParameterType { get; }
    public ReferenceType ReturnType { get; }

    public ConstructorSymbol(
        ObjectTypeSymbol containingSymbol,
        SimpleName? name,
        ReferenceType selfParameterType,
        IFixedList<Parameter> parameterTypes)
        : base(containingSymbol, name, parameterTypes,
            new Return(containingSymbol.DeclaresType.ToConstructorReturn(selfParameterType, parameterTypes)))
    {
        ContainingSymbol = containingSymbol;
        SelfParameterType = selfParameterType;
        ReturnType = (ReferenceType)base.Return.Type;
    }

    public static ConstructorSymbol CreateDefault(ObjectTypeSymbol containingSymbol)
        => new(containingSymbol, null, containingSymbol.DeclaresType.ToDefaultConstructorSelf(), FixedList.Empty<Parameter>());

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
        var name = Name is null ? $" {Name}" : "";
        var selfParameterType = new Parameter(false, SelfParameterType);
        return $"{ContainingSymbol}::new{name}({string.Join(", ", Parameters.Prepend(selfParameterType).Select(d => d.ToILString()))})";
    }
}
