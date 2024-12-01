using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class BuiltInTypeSymbol : TypeSymbol
{
    public override PackageSymbol? Package => null;
    public override Symbol? ContainingSymbol => null;
    public override TypeSymbol? ContextTypeSymbol => null;
    public override SpecialTypeName Name { get; }
    public DeclaredType DeclaresType { get; }

    public BuiltInTypeSymbol(AnyType declaresType)
        : base(declaresType.Name)
    {
        Name = declaresType.Name;
        DeclaresType = declaresType;
    }

    public BuiltInTypeSymbol(SimpleType declaresType)
        : base(declaresType.Name)
    {
        Name = declaresType.Name;
        DeclaresType = declaresType;
    }

    public override DeclaredType TryGetDeclaredType() => DeclaresType;

    #region Equality
    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is BuiltInTypeSymbol otherType
               && Name == otherType.Name
               && DeclaresType == otherType.DeclaresType;
    }

    public override int GetHashCode() => HashCode.Combine(Name, DeclaresType);
    #endregion

    public override string ToILString() => Name.ToString();
}
