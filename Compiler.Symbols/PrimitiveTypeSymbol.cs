using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public class PrimitiveTypeSymbol : TypeSymbol
{
    public override PackageSymbol? Package => null;
    public override Symbol? ContainingSymbol => null;
    public override SpecialTypeName Name { get; }

    public DataType DeclaresType { get; }

    public PrimitiveTypeSymbol(SimpleType declaresType)
        : base(declaresType.Name)
    {
        Name = declaresType.Name;
        DeclaresType = declaresType;
    }

    public PrimitiveTypeSymbol(EmptyType declaresType)
        : base(declaresType.Name)
    {
        Name = declaresType.Name;
        DeclaresType = declaresType;
    }

    /// <remarks>This is a little odd that it "declares" a specific reference type while the
    /// <see cref="ObjectTypeSymbol"/> declares a <see cref="DeclaredObjectType"/>.</remarks>
    public PrimitiveTypeSymbol(AnyType declaresType)
        : base(declaresType.Name)
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
