using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

[Closed(typeof(INonVoidType), typeof(EmptyType))]
public interface IType : IExpressionType, IMaybeType
{
    #region Standard Types
    public new static readonly UnknownType Unknown = UnknownType.Instance;
    public static readonly VoidType Void = VoidType.Instance;
    public static readonly NeverType Never = NeverType.Instance;
    public static readonly CapabilityType<BoolType> Bool = DeclaredType.Bool.Type;
    public static readonly OptionalType OptionalBool = new(Bool);
    public static readonly BoolConstValueType True = BoolConstValueType.True;
    public static readonly BoolConstValueType False = BoolConstValueType.False;
    public static readonly CapabilityType<BigIntegerType> Int = DeclaredType.Int.Type;
    public static readonly CapabilityType<BigIntegerType> UInt = DeclaredType.UInt.Type;
    public static readonly CapabilityType<FixedSizeIntegerType> Int8 = DeclaredType.Int8.Type;
    public static readonly CapabilityType<FixedSizeIntegerType> Byte = DeclaredType.Byte.Type;
    public static readonly CapabilityType<FixedSizeIntegerType> Int16 = DeclaredType.Int16.Type;
    public static readonly CapabilityType<FixedSizeIntegerType> UInt16 = DeclaredType.UInt16.Type;
    public static readonly CapabilityType<FixedSizeIntegerType> Int32 = DeclaredType.Int32.Type;
    public static readonly CapabilityType<FixedSizeIntegerType> UInt32 = DeclaredType.UInt32.Type;
    public static readonly CapabilityType<FixedSizeIntegerType> Int64 = DeclaredType.Int64.Type;
    public static readonly CapabilityType<FixedSizeIntegerType> UInt64 = DeclaredType.UInt64.Type;
    public static readonly CapabilityType<PointerSizedIntegerType> Size = DeclaredType.Size.Type;
    public static readonly CapabilityType<PointerSizedIntegerType> Offset = DeclaredType.Offset.Type;
    public static readonly CapabilityType<PointerSizedIntegerType> NInt = DeclaredType.NInt.Type;
    public static readonly CapabilityType<PointerSizedIntegerType> NUInt = DeclaredType.NUInt.Type;

    /// <summary>
    /// The value `none` has this type, which is `never?`.
    /// </summary>
    public static readonly OptionalType None = new(Never);
    #endregion

    public new IPlainType ToPlainType();
    IMaybePlainType IMaybeType.ToPlainType() => ToPlainType();

    /// <summary>
    /// Return the type for when a value of this type is accessed via a reference with the given capability.
    /// </summary>
    /// <remarks>This can restrict the ability to write to the value.</remarks>
    new IType AccessedVia(ICapabilityConstraint capability);
    IExpressionType IExpressionType.AccessedVia(ICapabilityConstraint capability) => AccessedVia(capability);
}
