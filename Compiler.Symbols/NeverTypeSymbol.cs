using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class NeverTypeSymbol : TypeSymbol
{
    #region Singleton
    public static readonly NeverTypeSymbol Instance = new NeverTypeSymbol();

    private NeverTypeSymbol() : base(BuiltInTypeName.Never) { }
    #endregion

    public override PackageFacetSymbol? Facet => null;
    public override Symbol? ContainingSymbol => null;
    public override TypeSymbol? ContextTypeSymbol => null;
    public override BuiltInTypeName Name => PlainType.Name;
    public NeverPlainType PlainType => Types.Plain.PlainType.Never;

    public override PlainType TryGetPlainType() => PlainType;

    #region Equality
    public override bool Equals(Symbol? other)
        // This is a singleton, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(typeof(NeverTypeSymbol));
    #endregion

    public override string ToILString() => Name.ToString();
}
