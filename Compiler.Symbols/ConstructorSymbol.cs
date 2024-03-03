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
    public override SimpleName? Name { get; }
    public ReferenceType SelfParameterType { get; }
    public ReferenceType ReturnType { get; }

    public ConstructorSymbol(
        UserTypeSymbol containingSymbol,
        SimpleName? name,
        ReferenceType selfParameterType,
        IFixedList<Parameter> parameterTypes)
        : base(parameterTypes,
            new Return(((ObjectType)containingSymbol.DeclaresType).ToConstructorReturn(selfParameterType, parameterTypes)))
    {
        ContainingSymbol = containingSymbol;
        Name = name;
        SelfParameterType = selfParameterType;
        ReturnType = (ReferenceType)base.Return.Type;
    }

    public static ConstructorSymbol CreateDefault(UserTypeSymbol containingSymbol)
        => new(containingSymbol, null,
            ((ObjectType)containingSymbol.DeclaresType).ToDefaultConstructorSelf(),
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

    public override int GetHashCode()
        => HashCode.Combine(ContainingSymbol, Name, Parameters);

    public override string ToILString()
    {
        var name = Name is null ? $".{Name}" : "";
        var selfParameterType = new Parameter(false, SelfParameterType);
        return $"{ContainingSymbol}::new{name}({string.Join(", ", Parameters.Prepend(selfParameterType).Select(d => d.ToILString()))})";
    }
}
