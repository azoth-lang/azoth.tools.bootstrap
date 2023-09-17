using System;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public sealed class BoolConstantType : BoolType
{
    internal new static readonly BoolConstantType True = new(true);
    internal new static readonly BoolConstantType False = new(false);

    public override bool IsTypeOfConstant => true;

    public bool Value { get; }

    private BoolConstantType(bool value)
        : base(value ? SpecialTypeName.True : SpecialTypeName.False)
    {
        Value = value;
    }

    public BoolConstantType And(BoolConstantType other) => Value && other.Value;
    public BoolConstantType Or(BoolConstantType other) => Value || other.Value;
    public BoolConstantType Not() => !Value;
    public BoolConstantType Equals(BoolConstantType other) => Value == other.Value;
    public BoolConstantType NotEquals(BoolConstantType other) => Value != other.Value;

    public static implicit operator BoolConstantType(bool value) => value ? True : False;

    public override DataType ToNonConstantType() => Bool;

    public override string ToSourceCodeString()
        => throw new InvalidOperationException("Bool constant type has no source code representation");

    public override string ToILString() => $"const[{(Value ? "true" : "false")}]";
}
