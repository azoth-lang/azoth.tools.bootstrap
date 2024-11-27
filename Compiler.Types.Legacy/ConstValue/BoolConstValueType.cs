using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.ConstValue;

/// <summary>
/// This is the type of a boolean constant value, it isn't possible to declare a variable of this
/// type.
/// </summary>
public sealed class BoolConstValueType : ConstValueType
{
    internal static readonly BoolConstValueType True = new(true);
    internal static readonly BoolConstValueType False = new(false);

    public bool Value { get; }

    private BoolConstValueType(bool value)
        : base(value ? SpecialTypeName.True : SpecialTypeName.False)
    {
        Value = value;
    }

    public BoolConstValueType And(BoolConstValueType other) => Value && other.Value;
    public BoolConstValueType Or(BoolConstValueType other) => Value || other.Value;
    public BoolConstValueType Not() => !Value;
    public BoolConstValueType Equals(BoolConstValueType other) => Value == other.Value;
    public BoolConstValueType NotEquals(BoolConstValueType other) => Value != other.Value;

    public static implicit operator BoolConstValueType(bool value) => value ? True : False;

    public override CapabilityType<BoolType> ToNonConstValueType() => IType.Bool;

    public override IMaybeExpressionAntetype ToAntetype()
        => Value ? IExpressionAntetype.True : IExpressionAntetype.False;

    public override string ToSourceCodeString()
        => throw new InvalidOperationException("Bool value type has no source code representation");

    public override string ToILString() => $"Value[{(Value ? "true" : "false")}]";
}
