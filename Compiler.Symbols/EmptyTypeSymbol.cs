using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class EmptyTypeSymbol : TypeSymbol
{
    public override PackageSymbol? Package => null;
    public override Symbol? ContainingSymbol => null;
    public override TypeSymbol? ContextTypeSymbol => null;
    public override SpecialTypeName Name { get; }
    public EmptyType Type { get; }

    public EmptyTypeSymbol(EmptyType type)
        : base(type.Name)
    {
        Name = type.Name;
        Type = type;
    }

    public override DataType GetDataType() => Type;

    #region Equality
    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is EmptyTypeSymbol otherType
               && Name == otherType.Name
               && Type.Equals(otherType.Type);
    }

    public override int GetHashCode() => HashCode.Combine(Name, Type);
    #endregion

    public override string ToILString() => Name.ToString();
}
