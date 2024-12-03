using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

// TODO this isn't right. The value should be a type parameter
public sealed class BoolLiteralTypeConstructor : LiteralTypeConstructor
{
    internal static readonly BoolLiteralTypeConstructor True = new(true);
    internal static readonly BoolLiteralTypeConstructor False = new(false);

    public bool Value { get; }
    public override OrdinaryNamedPlainType PlainType { get; }
    private BoolLiteralTypeConstructor(bool value)
        : base(SpecialTypeName.Bool)
    {
        Value = value;
        PlainType = new(this, []);
    }

    public override ITypeConstructor ToNonLiteral() => ITypeConstructor.Bool;

    public static implicit operator BoolLiteralTypeConstructor(bool value) => value ? True : False;

    #region Operations
    public BoolLiteralTypeConstructor Equals(BoolLiteralTypeConstructor other) => Value == other.Value;

    public BoolLiteralTypeConstructor NotEquals(BoolLiteralTypeConstructor other) => Value != other.Value;

    public BoolLiteralTypeConstructor And(BoolLiteralTypeConstructor other) => Value && other.Value;

    public BoolLiteralTypeConstructor Or(BoolLiteralTypeConstructor other) => Value || other.Value;

    public BoolLiteralTypeConstructor Not() => Value ? False : True;
    #endregion

    #region Equality
    public override bool Equals(ITypeConstructor? other)
        // Bool literal values are singletons, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(Value);
    #endregion

    public override string ToString() => $"bool[{(Value ? "true" : "false")}]";
}
