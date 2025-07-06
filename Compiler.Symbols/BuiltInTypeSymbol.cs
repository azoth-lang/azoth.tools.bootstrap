using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class BuiltInTypeSymbol : TypeSymbol
{
    public override PackageFacetSymbol? Facet => null;
    public override Symbol? ContainingSymbol => null;
    public override TypeSymbol? ContextTypeSymbol => null;
    public override BuiltInTypeName Name { get; }
    public BareTypeConstructor TypeConstructor { get; }

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

    public override BareTypeConstructor TryGetTypeConstructor() => TypeConstructor;

    #region Equality
    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is BuiltInTypeSymbol otherType
               && TypeConstructor.Equals(otherType.TypeConstructor);
    }

    public override int GetHashCode() => HashCode.Combine(TypeConstructor);
    #endregion

    public override string ToILString() => Name.ToString();
}
