using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public class PrimitiveTypeSymbol : TypeSymbol
{
    public new SpecialTypeName Name { get; }

    public DataType DeclaresType { get; }

    public PrimitiveTypeSymbol(SimpleType declaresType)
        : base(null, declaresType.Name)
    {
        Name = declaresType.Name;
        DeclaresType = declaresType;
    }

    public PrimitiveTypeSymbol(EmptyType declaresType)
        : base(null, declaresType.Name)
    {
        Name = declaresType.Name;
        DeclaresType = declaresType;
    }

    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is PrimitiveTypeSymbol otherType
               && Name == otherType.Name
               && DeclaresType == otherType.DeclaresType;
    }

    public override int GetHashCode() => HashCode.Combine(Name, DeclaresType);

    public override string ToILString() => Name.ToString();
}
