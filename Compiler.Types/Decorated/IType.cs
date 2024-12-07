using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

[Closed(typeof(INonVoidType), typeof(VoidType))]
public interface IType : IMaybeType
{
    #region Standard Types
    public static readonly UnknownType Unknown = UnknownType.Instance;
    public static readonly VoidType Void = VoidType.Instance;
    public static readonly NeverType Never = NeverType.Instance;
    public static readonly CapabilityType Bool = new(Capability.Constant, IPlainType.Bool);
    public static readonly OptionalType OptionalBool = new(IPlainType.OptionalBool, Bool);
    public static readonly CapabilityType Int = new(Capability.Constant, IPlainType.Int);
    public static readonly CapabilityType UInt = new(Capability.Constant, IPlainType.UInt);
    public static readonly CapabilityType Int8 = new(Capability.Constant, IPlainType.Int8);
    public static readonly CapabilityType Byte = new(Capability.Constant, IPlainType.Byte);
    public static readonly CapabilityType Int16 = new(Capability.Constant, IPlainType.Int16);
    public static readonly CapabilityType UInt16 = new(Capability.Constant, IPlainType.UInt16);
    public static readonly CapabilityType Int32 = new(Capability.Constant, IPlainType.Int32);
    public static readonly CapabilityType UInt32 = new(Capability.Constant, IPlainType.UInt32);
    public static readonly CapabilityType Int64 = new(Capability.Constant, IPlainType.Int64);
    public static readonly CapabilityType UInt64 = new(Capability.Constant, IPlainType.UInt64);
    public static readonly CapabilityType Size = new(Capability.Constant, IPlainType.Size);
    public static readonly CapabilityType Offset = new(Capability.Constant, IPlainType.Offset);
    public static readonly CapabilityType NInt = new(Capability.Constant, IPlainType.NInt);
    public static readonly CapabilityType NUInt = new(Capability.Constant, IPlainType.NUInt);
    #endregion

    #region Literal Types
    /// <summary>
    /// The value `none` has this type, which is `never?`.
    /// </summary>
    public static readonly OptionalType None = new(IPlainType.None, Never);

    public static readonly CapabilityType True = new(Capability.Constant, IPlainType.True);
    public static readonly CapabilityType False = new(Capability.Constant, IPlainType.False);
    #endregion

    new IPlainType PlainType { get; }
    IMaybePlainType IMaybeType.PlainType => PlainType;
}
