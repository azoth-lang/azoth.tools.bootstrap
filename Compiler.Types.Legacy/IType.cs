using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;
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
    public static readonly CapabilityType Bool = DeclaredType.Bool.Type;
    public static readonly OptionalType OptionalBool = new(Bool);
    public static readonly BoolConstValueType True = BoolConstValueType.True;
    public static readonly BoolConstValueType False = BoolConstValueType.False;
    public static readonly CapabilityType Int = DeclaredType.Int.Type;
    public static readonly CapabilityType UInt = DeclaredType.UInt.Type;
    public static readonly CapabilityType Int8 = DeclaredType.Int8.Type;
    public static readonly CapabilityType Byte = DeclaredType.Byte.Type;
    public static readonly CapabilityType Int16 = DeclaredType.Int16.Type;
    public static readonly CapabilityType UInt16 = DeclaredType.UInt16.Type;
    public static readonly CapabilityType Int32 = DeclaredType.Int32.Type;
    public static readonly CapabilityType UInt32 = DeclaredType.UInt32.Type;
    public static readonly CapabilityType Int64 = DeclaredType.Int64.Type;
    public static readonly CapabilityType UInt64 = DeclaredType.UInt64.Type;
    public static readonly CapabilityType Size = DeclaredType.Size.Type;
    public static readonly CapabilityType Offset = DeclaredType.Offset.Type;
    public static readonly CapabilityType NInt = DeclaredType.NInt.Type;
    public static readonly CapabilityType NUInt = DeclaredType.NUInt.Type;

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
}
