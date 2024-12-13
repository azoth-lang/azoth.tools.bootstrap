using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

[Closed(typeof(INonVoidType), typeof(VoidType))]
public interface IType : IMaybeType
{
    #region Standard Types
    public static new readonly UnknownType Unknown = UnknownType.Instance;
    public static readonly VoidType Void = VoidType.Instance;
    public static readonly NeverType Never = NeverType.Instance;
    public static readonly CapabilityType IdAny = CapabilityType.Create(Capability.Identity, IPlainType.Any);
    public static readonly CapabilityType Bool = TypeConstructor.Bool.Type;
    public static readonly OptionalType OptionalBool = new(IPlainType.OptionalBool, Bool);
    public static readonly CapabilityType Int = TypeConstructor.Int.Type;
    public static readonly CapabilityType UInt = TypeConstructor.UInt.Type;
    public static readonly CapabilityType Int8 = TypeConstructor.Int8.Type;
    public static readonly CapabilityType Byte = TypeConstructor.Byte.Type;
    public static readonly CapabilityType Int16 = TypeConstructor.Int16.Type;
    public static readonly CapabilityType UInt16 = TypeConstructor.UInt16.Type;
    public static readonly CapabilityType Int32 = TypeConstructor.Int32.Type;
    public static readonly CapabilityType UInt32 = TypeConstructor.UInt32.Type;
    public static readonly CapabilityType Int64 = TypeConstructor.Int64.Type;
    public static readonly CapabilityType UInt64 = TypeConstructor.UInt64.Type;
    public static readonly CapabilityType Size = TypeConstructor.Size.Type;
    public static readonly CapabilityType Offset = TypeConstructor.Offset.Type;
    public static readonly CapabilityType NInt = TypeConstructor.NInt.Type;
    public static readonly CapabilityType NUInt = TypeConstructor.NUInt.Type;
    #endregion

    #region Literal Types
    /// <summary>
    /// The value `none` has this type, which is `never?`.
    /// </summary>
    public static readonly OptionalType None = new(IPlainType.None, Never);

    public static readonly CapabilityType True = TypeConstructor.True.Type;
    public static readonly CapabilityType False = TypeConstructor.False.Type;
    #endregion

    new IPlainType PlainType { get; }
    IMaybePlainType IMaybeType.PlainType => PlainType;

    /// <summary>
    /// Convert types for literals (e.g. <c>bool[true]</c>, <c>int[42]</c> etc.) to their
    /// corresponding types.
    /// </summary>
    // TODO this makes literal types special. Perhaps there should be a way to declare other literal types in code
    new IType ToNonLiteral();
    IMaybeType IMaybeType.ToNonLiteral() => ToNonLiteral();
}
