using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class VoidTypeSymbol : TypeSymbol
{
    #region Singleton
    public static readonly VoidTypeSymbol Instance = new VoidTypeSymbol();

    private VoidTypeSymbol() : base(BuiltInTypeName.Void) { }
    #endregion

    public override PackageFacetSymbol? Facet => null;
    public override Symbol? ContainingSymbol => null;
    public override TypeSymbol? ContextTypeSymbol => null;
    public override BuiltInTypeName Name => PlainType.Name;
    public VoidPlainType PlainType => Types.Plain.PlainType.Void;

    public override VoidPlainType TryGetPlainType() => PlainType;

    #region Equality
    public override bool Equals(Symbol? other)
        // This is a singleton, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(typeof(VoidTypeSymbol));
    #endregion

    public override string ToILString() => Name.ToString();
}
