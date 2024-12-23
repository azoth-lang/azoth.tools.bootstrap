using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

// TODO this isn't right. The value should be a type parameter
public sealed class BoolLiteralTypeConstructor : LiteralTypeConstructor
{
    internal new static readonly BoolLiteralTypeConstructor True = new(true);
    internal new static readonly BoolLiteralTypeConstructor False = new(false);

    public bool Value { [DebuggerStepThrough] get; }
    public override BarePlainType PlainType { get; }
    private BoolLiteralTypeConstructor(bool value)
        : base(BuiltInTypeName.Bool)
    {
        Value = value;
        PlainType = new(this, containingType: null, []);
    }

    public override BoolTypeConstructor TryToNonLiteral() => Bool;

    #region Equality
    public override bool Equals(BareTypeConstructor? other)
        // Bool literal values are singletons, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(Value);
    #endregion

    public override string ToString() => $"bool[{(Value ? "true" : "false")}]";
}
