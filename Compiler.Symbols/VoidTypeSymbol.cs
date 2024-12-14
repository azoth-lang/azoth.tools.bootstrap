using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class VoidTypeSymbol : TypeSymbol
{
    #region Singleton
    public static readonly VoidTypeSymbol Instance = new VoidTypeSymbol();

    private VoidTypeSymbol() : base(SpecialTypeName.Void) { }
    #endregion

    public override PackageSymbol? Package => null;
    public override Symbol? ContainingSymbol => null;
    public override TypeSymbol? ContextTypeSymbol => null;
    public override SpecialTypeName Name => PlainType.Name;
    public VoidPlainType PlainType => Types.Plain.PlainType.Void;

    public override VoidPlainType TryGetPlainType() => PlainType;
    public override Type? TryGetType() => Type.Void;

    #region Equality
    public override bool Equals(Symbol? other)
        // This is a singleton, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(typeof(VoidTypeSymbol));
    #endregion

    public override string ToILString() => Name.ToString();
}
