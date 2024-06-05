using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes.ConstValue;

public sealed class BoolConstValueAntetype : ConstValueAntetype
{
    internal static readonly BoolConstValueAntetype True = new(true);
    internal static readonly BoolConstValueAntetype False = new(false);

    public bool Value { get; }

    private BoolConstValueAntetype(bool value)
        : base(value ? SpecialTypeName.True : SpecialTypeName.False)
    {
        Value = value;
    }

    public override IMaybeAntetype ToNonConstValueType() => IAntetype.Bool;

    #region Equality
    public override bool Equals(IMaybeExpressionAntetype? other)
        // Bool const values are singletons, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(Value);
    #endregion
}
