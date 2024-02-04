using System;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public sealed class BoolValueType : BoolType
{
    internal new static readonly BoolValueType True = new(true);
    internal new static readonly BoolValueType False = new(false);

    public override bool IsTypeOfValue => true;

    public bool Value { get; }

    private BoolValueType(bool value)
        : base(value ? SpecialTypeName.True : SpecialTypeName.False)
    {
        Value = value;
    }

    public BoolValueType And(BoolValueType other) => Value && other.Value;
    public BoolValueType Or(BoolValueType other) => Value || other.Value;
    public BoolValueType Not() => !Value;
    public BoolValueType Equals(BoolValueType other) => Value == other.Value;
    public BoolValueType NotEquals(BoolValueType other) => Value != other.Value;

    public static implicit operator BoolValueType(bool value) => value ? True : False;

    public override DataType ToNonConstantType() => Bool;

    public override string ToSourceCodeString()
        => throw new InvalidOperationException("Bool value type has no source code representation");

    public override string ToILString() => $"Value[{(Value ? "true" : "false")}]";
}
