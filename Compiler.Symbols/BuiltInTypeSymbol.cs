using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class BuiltInTypeSymbol : TypeSymbol
{
    public override PackageSymbol? Package => null;
    public override Symbol? ContainingSymbol => null;
    public override TypeSymbol? ContextTypeSymbol => null;
    public override SpecialTypeName Name { get; }
    public TypeConstructor TypeConstructor { get; }

    public BuiltInTypeSymbol(AnyTypeConstructor typeConstructor)
        : base(typeConstructor.Name)
    {
        Name = typeConstructor.Name;
        TypeConstructor = typeConstructor;
    }

    public BuiltInTypeSymbol(SimpleTypeConstructor typeConstructor)
        : base(typeConstructor.Name)
    {
        Name = typeConstructor.Name;
        TypeConstructor = typeConstructor;
    }

    public override TypeConstructor TryGetTypeConstructor() => TypeConstructor;

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
