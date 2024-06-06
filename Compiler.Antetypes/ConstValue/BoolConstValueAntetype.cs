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

    public static implicit operator BoolConstValueAntetype(bool value) => value ? True : False;

    #region Operations
    public BoolConstValueAntetype Equals(BoolConstValueAntetype other) => Value == other.Value;

    public BoolConstValueAntetype NotEquals(BoolConstValueAntetype other) => Value != other.Value;

    public BoolConstValueAntetype And(BoolConstValueAntetype other) => Value && other.Value;

    public BoolConstValueAntetype Or(BoolConstValueAntetype other) => Value || other.Value;
    #endregion

    #region Equality
    public override bool Equals(IMaybeExpressionAntetype? other)
        // Bool const values are singletons, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(Value);
    #endregion

    public override string ToString() => $"Value[{(Value ? "true" : "false")}]";
}
