using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

public sealed class BoolLiteralTypeConstructor : LiteralTypeConstructor
{
    internal static readonly BoolLiteralTypeConstructor True = new(true);
    internal static readonly BoolLiteralTypeConstructor False = new(false);

    public bool Value { get; }

    private BoolLiteralTypeConstructor(bool value)
        : base(value ? SpecialTypeName.True : SpecialTypeName.False)
    {
        Value = value;
    }

    public override IMaybeAntetype ToNonConstValueType() => IAntetype.Bool;

    public static implicit operator BoolLiteralTypeConstructor(bool value) => value ? True : False;

    #region Operations
    public BoolLiteralTypeConstructor Equals(BoolLiteralTypeConstructor other) => Value == other.Value;

    public BoolLiteralTypeConstructor NotEquals(BoolLiteralTypeConstructor other) => Value != other.Value;

    public BoolLiteralTypeConstructor And(BoolLiteralTypeConstructor other) => Value && other.Value;

    public BoolLiteralTypeConstructor Or(BoolLiteralTypeConstructor other) => Value || other.Value;

    public BoolLiteralTypeConstructor Not() => Value ? False : True;
    #endregion

    #region Equality
    public override bool Equals(IMaybeExpressionAntetype? other)
        // Bool const values are singletons, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(Value);
    #endregion

    public override string ToString() => $"bool[{(Value ? "true" : "false")}]";
}
