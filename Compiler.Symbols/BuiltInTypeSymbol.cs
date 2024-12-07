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
    public DeclaredType TypeConstructor { get; }

    public BuiltInTypeSymbol(AnyType typeConstructor)
        : base(typeConstructor.Name)
    {
        Name = typeConstructor.Name;
        TypeConstructor = typeConstructor;
    }

    public BuiltInTypeSymbol(SimpleType typeConstructor)
        : base(typeConstructor.Name)
    {
        Name = typeConstructor.Name;
        TypeConstructor = typeConstructor;
    }

    public override DeclaredType TryGetTypeConstructor() => TypeConstructor;

    #region Equality
    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is BuiltInTypeSymbol otherType
               && Name == otherType.Name
               && TypeConstructor == otherType.TypeConstructor;
    }

    public override int GetHashCode() => HashCode.Combine(Name, TypeConstructor);
    #endregion

    public override string ToILString() => Name.ToString();
}
