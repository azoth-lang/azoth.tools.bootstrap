using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class NeverTypeSymbol : TypeSymbol
{
    #region Singleton
    public static readonly NeverTypeSymbol Instance = new NeverTypeSymbol();

    private NeverTypeSymbol() : base(SpecialTypeName.Never) { }
    #endregion

    public override PackageSymbol? Package => null;
    public override Symbol? ContainingSymbol => null;
    public override TypeSymbol? ContextTypeSymbol => null;
    public override SpecialTypeName Name => PlainType.Name;
    public NeverPlainType PlainType => IPlainType.Never;

    public override IPlainType TryGetPlainType() => PlainType;
    public override IType? TryGetType() => IType.Never;

    #region Equality
    public override bool Equals(Symbol? other)
        // This is a singleton, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(typeof(NeverTypeSymbol));
    #endregion

    public override string ToILString() => Name.ToString();
}
