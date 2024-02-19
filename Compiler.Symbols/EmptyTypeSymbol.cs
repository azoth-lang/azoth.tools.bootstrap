using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class EmptyTypeSymbol : TypeSymbol
{
    public override PackageSymbol? Package => null;
    public override Symbol? ContainingSymbol => null;
    public override SpecialTypeName Name { get; }
    public EmptyType DeclaresType { get; }

    public EmptyTypeSymbol(EmptyType declaresType)
        : base(declaresType.Name)
    {
        Name = declaresType.Name;
        DeclaresType = declaresType;
    }

    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is EmptyTypeSymbol otherType
               && Name == otherType.Name
               && DeclaresType == otherType.DeclaresType;
    }

    public override int GetHashCode() => HashCode.Combine(Name, DeclaresType);

    public override string ToILString() => Name.ToString();
}
