using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

[Closed(typeof(INonVoidType), typeof(EmptyType))]
public interface IType : IPseudotype, IMaybeType
{
    #region Standard Types
    public new static readonly UnknownType Unknown = UnknownType.Instance;
    public static readonly VoidType Void = VoidType.Instance;
    public static readonly NeverType Never = NeverType.Instance;
    public static readonly CapabilityType Bool = new(Capability.Constant, BareNonVariableType.Bool);
    public static readonly OptionalType OptionalBool = new(Bool);
    public static readonly CapabilityType True = new(Capability.Constant, BareNonVariableType.True);
    public static readonly CapabilityType False = new(Capability.Constant, BareNonVariableType.False);
    public static readonly CapabilityType Int = new(Capability.Constant, BareNonVariableType.Int);
    public static readonly CapabilityType UInt = new(Capability.Constant, BareNonVariableType.UInt);
    public static readonly CapabilityType Int8 = new(Capability.Constant, BareNonVariableType.Int8);
    public static readonly CapabilityType Byte = new(Capability.Constant, BareNonVariableType.Byte);
    public static readonly CapabilityType Int16 = new(Capability.Constant, BareNonVariableType.Int16);
    public static readonly CapabilityType UInt16 = new(Capability.Constant, BareNonVariableType.UInt16);
    public static readonly CapabilityType Int32 = new(Capability.Constant, BareNonVariableType.Int32);
    public static readonly CapabilityType UInt32 = new(Capability.Constant, BareNonVariableType.UInt32);
    public static readonly CapabilityType Int64 = new(Capability.Constant, BareNonVariableType.Int64);
    public static readonly CapabilityType UInt64 = new(Capability.Constant, BareNonVariableType.UInt64);
    public static readonly CapabilityType Size = new(Capability.Constant, BareNonVariableType.Size);
    public static readonly CapabilityType Offset = new(Capability.Constant, BareNonVariableType.Offset);
    public static readonly CapabilityType NInt = new(Capability.Constant, BareNonVariableType.NInt);
    public static readonly CapabilityType NUInt = new(Capability.Constant, BareNonVariableType.NUInt);

    /// <summary>
    /// The value `none` has this type, which is `never?`.
    /// </summary>
    public static readonly OptionalType None = new(Never);
    #endregion

    public new IPlainType ToPlainType();
    IMaybePlainType IMaybeType.ToPlainType() => ToPlainType();

    /// <summary>
    /// Convert types for constant values to their corresponding types.
    /// </summary>
    new IType ToNonConstValueType();
    IMaybeType IMaybeType.ToNonConstValueType() => ToNonConstValueType();

    /// <summary>
    /// Return the type for when a value of this type is accessed via a reference with the given capability.
    /// </summary>
    /// <remarks>This can restrict the ability to write to the value.</remarks>
    new IType AccessedVia(ICapabilityConstraint capability);
    IMaybeType IMaybeType.AccessedVia(ICapabilityConstraint capability) => AccessedVia(capability);

    Decorated.IType ToDecoratedType();
}
