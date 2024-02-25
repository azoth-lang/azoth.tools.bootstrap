using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public static class DataTypeExtensions
{
    /// <summary>
    /// Tests whether a place of the target type could be assigned a value of the source type.
    /// This does not account for implicit conversions, but does allow for borrowing
    /// and sharing. It also allows for isolated upgrading to mutable.
    /// </summary>
    public static bool IsAssignableFrom(this DataType target, DataType source)
    {
        return (target, source) switch
        {
            (_, _) when target.Equals(source) => true,
            (UnknownType, _) or (_, UnknownType) or (_, NeverType)
                => true,
            (ValueType t, ValueType s)
                => IsAssignableFrom(t, s),
            (ReferenceType targetReference, ReferenceType sourceReference)
                => IsAssignableFrom(targetReference, sourceReference),
            (OptionalType targetOptional, OptionalType sourceOptional)
                => IsAssignableFrom(targetOptional.Referent, sourceOptional.Referent),
            (OptionalType targetOptional, _)
                => IsAssignableFrom(targetOptional.Referent, source),
            (FunctionType targetFunction, FunctionType sourceFunction)
                => IsAssignableFrom(targetFunction, sourceFunction),
            _ => false
        };
    }

    public static bool IsAssignableFrom(this ValueType target, ValueType source)
        //if (!target.Capability.IsAssignableFrom(source.Capability)) return false;
        => IsAssignableFrom(target.BareType, source.BareType);

    public static bool IsAssignableFrom(this ReferenceType target, ReferenceType source)
    {
        if (!target.Capability.IsAssignableFrom(source.Capability)) return false;

        if (IsAssignableFrom(target.BareType, target.AllowsWrite, source.BareType))
            return true;

        // TODO remove hack to allow string to exist in both primitives and stdlib
        return target.Name == "String" && source.Name == "String"
            && target.ContainingNamespace == NamespaceName.Global
            && source.ContainingNamespace == NamespaceName.Global;
    }

    public static bool IsAssignableFrom(this BareType target, bool targetAllowsWrite, BareType source)
    {
        return target switch
        {
            BareReferenceType t => t.IsAssignableFrom(targetAllowsWrite, source),
            BareValueType t => t.IsAssignableFrom(source),
            _ => throw ExhaustiveMatch.Failed(target)
        };
    }

    /// <remarks>We currently support implicit boxing, so any bare type with the correct supertype
    /// is assignable.</remarks>
    public static bool IsAssignableFrom(
        this BareReferenceType target,
        bool targetAllowsWrite,
        BareType source)
    {
        if (source.Equals(target) || source.Supertypes.Contains(target))
            return true;

        if (target.AllowsVariance)
        {
            var declaredType = target.DeclaredType;
            var matchingDeclaredType = source.Supertypes.Prepend(source).Where(t => t.DeclaredType == declaredType);
            foreach (var sourceType in matchingDeclaredType)
                if (IsAssignableFrom(declaredType, targetAllowsWrite, target.GenericTypeArguments, sourceType.GenericTypeArguments))
                    return true;
        }

        return false;
    }

    public static bool IsAssignableFrom(this BareValueType target, BareType source)
        // Because a value type is never the supertype, we only need to check for equality.
        => source.Equals(target);

    private static bool IsAssignableFrom(
        DeclaredReferenceType declaredType,
        bool targetAllowsWrite,
        IFixedList<DataType> target,
        IFixedList<DataType> source)
    {
        for (int i = 0; i < declaredType.GenericParameters.Count; i++)
        {
            var from = source[i];
            var to = target[i];
            switch (declaredType.GenericParameters[i].ParameterVariance)
            {
                case ParameterVariance.Invariant:
                    if (from != to)
                        return false;
                    break;
                case ParameterVariance.Independent:
                    if (from != to)
                    {
                        // When target allows write, acts invariant
                        if (targetAllowsWrite)
                            return false;

                        if (from is not CapabilityType fromCapabilityType
                            || to is not CapabilityType toCapabilityType)
                            return false;

                        if (fromCapabilityType.BareType != toCapabilityType.BareType
                            // TODO does this handle `iso` and `id` correctly?
                            || !toCapabilityType.Capability.IsAssignableFrom(fromCapabilityType.Capability))
                            return false;
                    }
                    break;
                case ParameterVariance.SharableIndependent:
                    if (from != to)
                    {
                        // When target allows write, acts invariant
                        if (targetAllowsWrite) return false;

                        if (from is not CapabilityType fromCapabilityType || to is not CapabilityType toCapabilityType)
                            return false;

                        if (fromCapabilityType.BareType != toCapabilityType.BareType
                            // TODO does this handle `iso` and `id` correctly?
                            || !toCapabilityType.Capability.IsAssignableFrom(fromCapabilityType.Capability))
                            return false;

                        if (toCapabilityType.Capability == Capability.Identity
                           && fromCapabilityType.Capability == Capability.Constant)
                            return false;

                        // TODO what about `temp const`?
                    }
                    break;
                case ParameterVariance.Covariant:
                    if (!to.IsAssignableFrom(from))
                        return false;
                    break;
                case ParameterVariance.Contravariant:
                    if (!from.IsAssignableFrom(to))
                        return false;
                    break;
                default:
                    throw ExhaustiveMatch.Failed(declaredType.GenericParameters[i].ParameterVariance);
            }
        }
        return true;
    }

    public static bool IsAssignableFrom(this FunctionType target, FunctionType source)
    {
        if (target.Parameters.Count != source.Parameters.Count)
            return false;

        foreach (var (targetParameter, sourceParameter) in target.Parameters.Zip(source.Parameters))
            if (!targetParameter.IsAssignableFrom(sourceParameter))
                return false;

        return IsAssignableFrom(target.Return, source.Return);
    }

    public static bool IsAssignableFrom(this Parameter target, Parameter source)
    {
        // TODO add more flexibility in lent
        if (target.IsLent != source.IsLent) return false;

        // Parameter types need to be more specific in the target than the source.
        return source.Type.IsAssignableFrom(target.Type);
    }

    public static bool IsAssignableFrom(this Return target, Return source)
        // Return types need to be more general in the target than the source.
        => target.Type.IsAssignableFrom(source.Type);

    public static DataType ReplaceSelfWith(this DataType type, DataType selfType)
    {
        if (selfType is not ReferenceType selfReferenceType)
            return type;
        return type.ReplaceSelfWith(selfReferenceType.Capability);
    }

    public static DataType ReplaceSelfWith(this DataType type, Capability capability)
    {
        return type switch
        {
            SelfViewpointType t => t.Referent.ReplaceSelfWith(capability).AccessedVia(capability),
            //ReferenceType t => ReplaceSelfWith(t, capability),
            //OptionalType t => ReplaceSelfWith(t, capability),
            _ => type,
        };
    }

    //private static DataType ReplaceSelfWith(ReferenceType type, ReferenceCapability capability)
    //{
    //    var bareType = type.BareType.ReplaceSelfWith(capability);
    //    return ReferenceEquals(type.BareType, bareType) ? type : bareType.With(type.Capability);
    //}

    //private static DataType ReplaceSelfWith(OptionalType type, ReferenceCapability capability)
    //{
    //    var referent = type.Referent.ReplaceSelfWith(capability);
    //    return ReferenceEquals(type.Referent, referent) ? type : new OptionalType(type);
    //}

    //private static BareReferenceType ReplaceSelfWith(this BareReferenceType type, ReferenceCapability capability)
    //{
    //    // TODO data type visitor and replacement
    //    var typeArguments = type.TypeArguments.ReplaceSelfWith(capability);
    //    if (type is not SelfBareReferenceType selfType)
    //        return type;

    //    return selfType.With(capability);
    //}

    //private static BareReferenceType ReplaceSelfWith(this BareReferenceType type, ReferenceCapability capability)
    //{

    //}

    public static string ToILString(this IFixedList<DataType> types)
        => string.Join(", ", types.Select(t => t.ToILString()));

    /// <summary>
    /// If this is a reference type or an optional reference type, the underlying reference type.
    /// Otherwise, <see langword="null"/>.
    /// </summary>
    public static ReferenceType? UnderlyingReferenceType(this DataType type)
    {
        return type switch
        {
            ReferenceType referenceType => referenceType,
            OptionalType { Referent: ReferenceType referenceType } => referenceType,
            _ => null
        };
    }

    [return: NotNullIfNotNull(nameof(type))]
    public static DataType? ToOptional(this DataType? type)
        => type switch
        {
            null => null,
            UnknownType or NeverType or VoidType => type,
            _ => new OptionalType(type),
        };

    /// <summary>
    /// Determine what the common type for two numeric types for a numeric operator is.
    /// </summary>
    public static DataType? NumericOperatorCommonType(this DataType leftType, DataType rightType)
        => (leftType, rightType) switch
        {
            ({ IsFullyKnown: false }, _) => DataType.Unknown,
            (_, { IsFullyKnown: false }) => DataType.Unknown,
            (NonEmptyType left, NeverType) => left.AsNumericType()?.Type,
            (NeverType, NonEmptyType right) => right.AsNumericType()?.Type,
            (OptionalType { Referent: var left }, OptionalType { Referent: var right })
                => left.NumericOperatorCommonType(right).ToOptional(),
            (OptionalType { Referent: var left }, _) => left.NumericOperatorCommonType(rightType).ToOptional(),
            (_, OptionalType { Referent: var right }) => leftType.NumericOperatorCommonType(right).ToOptional(),
            (NonEmptyType left, NonEmptyType right)
                => left.AsNumericType()?.NumericOperatorCommonType(right.AsNumericType()),
            _ => null,
        };

    /// <summary>
    /// Determine what the common type for two numeric types for a numeric operator is.
    /// </summary>
    internal static DataType? NumericOperatorCommonType(this INumericType? leftType, INumericType? rightType)
        => (leftType, rightType) switch
        {
            (BigIntegerType left, IntegerType right)
                => left.IsSigned || right.IsSigned ? DataType.Int : DataType.UInt,
            (IntegerType left, BigIntegerType right)
                => left.IsSigned || right.IsSigned ? DataType.Int : DataType.UInt,
            (BigIntegerType left, IntegerConstValueType right)
                => left.IsSigned || right.IsSigned ? DataType.Int : DataType.UInt,
            (IntegerConstValueType left, BigIntegerType right)
                => left.IsSigned || right.IsSigned ? DataType.Int : DataType.UInt,
            (PointerSizedIntegerType left, PointerSizedIntegerType right)
                => left.IsSigned || right.IsSigned ? DataType.Offset : DataType.Size,
            (PointerSizedIntegerType { IsSigned: true }, IntegerConstValueType { IsInt16: true })
                or (PointerSizedIntegerType { IsSigned: false }, IntegerConstValueType { IsUInt16: true })
                => leftType.Type,
            (PointerSizedIntegerType left, IntegerConstValueType right)
                => left.IsSigned || right.IsSigned ? DataType.Int : DataType.UInt,
            (IntegerConstValueType { IsInt16: true }, PointerSizedIntegerType { IsSigned: true })
                or (IntegerConstValueType { IsUInt16: true }, PointerSizedIntegerType { IsSigned: false })
                => rightType.Type,
            (IntegerConstValueType left, PointerSizedIntegerType right)
                => left.IsSigned || right.IsSigned ? DataType.Int : DataType.UInt,
            (FixedSizeIntegerType left, FixedSizeIntegerType right)
                when left.IsSigned == right.IsSigned
                => (left.Bits >= right.Bits ? left : right).Type,
            (FixedSizeIntegerType { IsSigned: true } left, FixedSizeIntegerType right)
                when left.Bits > right.Bits
                => left.Type,
            (FixedSizeIntegerType left, FixedSizeIntegerType { IsSigned: true } right)
                when left.Bits < right.Bits
                => right.Type,
            (FixedSizeIntegerType { IsSigned: true } left, IntegerConstValueType right)
                when left.IsSigned || right.IsSigned
                => left.NumericOperatorCommonType(right.ToSmallestSignedIntegerType()),
            (FixedSizeIntegerType { IsSigned: false } left, IntegerConstValueType { IsSigned: false } right)
                => left.NumericOperatorCommonType(right.ToSmallestUnsignedIntegerType()),
            (IntegerConstValueType left, FixedSizeIntegerType right)
                when left.IsSigned || right.IsSigned
                => left.ToSmallestSignedIntegerType().NumericOperatorCommonType(right),
            (IntegerConstValueType { IsSigned: false } left, FixedSizeIntegerType { IsSigned: false } right)
                => left.ToSmallestSignedIntegerType().NumericOperatorCommonType(right),
            _ => null
        };

    private static INumericType? AsNumericType(this NonEmptyType type)
        => type switch
        {
            ValueType { DeclaredType: NumericType t } => t,
            IntegerConstValueType t => t,
            _ => null,
        };
}
