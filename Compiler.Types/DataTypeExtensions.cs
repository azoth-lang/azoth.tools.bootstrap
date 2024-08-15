using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
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
            (CapabilityType t, CapabilityType s)
                => IsAssignableFrom(t, s),
            (OptionalType targetOptional, OptionalType sourceOptional)
                => IsAssignableFrom(targetOptional.Referent, sourceOptional.Referent),
            (OptionalType targetOptional, _)
                => IsAssignableFrom(targetOptional.Referent, source),
            (FunctionType targetFunction, FunctionType sourceFunction)
                => IsAssignableFrom(targetFunction, sourceFunction),
            _ => false
        };
    }

    public static bool IsAssignableFrom(this CapabilityType target, CapabilityType source)
    {
        if (!target.Capability.IsAssignableFrom(source.Capability)) return false;

        if (IsAssignableFrom(target.BareType, target.AllowsWrite, source.BareType))
            return true;

        // TODO remove hack to allow string to exist in both primitives and stdlib
        return target.Name == "String" && source.Name == "String"
            && target.ContainingNamespace == NamespaceName.Global
            && source.ContainingNamespace == NamespaceName.Global;
    }

    /// <remarks>We currently support implicit boxing, so any bare type with the correct supertype
    /// is assignable.</remarks>
    public static bool IsAssignableFrom(
        this BareType target,
        bool targetAllowsWrite,
        BareType source)
    {
        if (source.Equals(target) || source.Supertypes.Contains(target))
            return true;

        if (target.AllowsVariance || target.HasIndependentTypeArguments)
        {
            var declaredType = target.DeclaredType;
            var matchingSourceTypes = source.Supertypes.Prepend(source).Where(t => t.DeclaredType == declaredType);
            foreach (var sourceType in matchingSourceTypes)
                if (IsAssignableFrom(declaredType, targetAllowsWrite, target.GenericTypeArguments, sourceType.GenericTypeArguments))
                    return true;
        }

        return false;
    }

    private static bool IsAssignableFrom(
        DeclaredType declaredType,
        bool targetAllowsWrite,
        IFixedList<DataType> target,
        IFixedList<DataType> source)
    {
        Requires.That(target.Count == declaredType.GenericParameters.Count, nameof(target), "count must match count of declaredType generic parameters");
        Requires.That(source.Count == target.Count, nameof(source), "count must match count of target");
        for (int i = 0; i < declaredType.GenericParameters.Count; i++)
        {
            var from = source[i];
            var to = target[i];
            var genericParameter = declaredType.GenericParameters[i];
            switch (genericParameter.Variance)
            {
                default:
                    throw ExhaustiveMatch.Failed(genericParameter.Variance);
                case ParameterVariance.Invariant:
                    if (from != to)
                    {
                        // When target allows write, acts invariant regardless of independence
                        if (targetAllowsWrite) return false;

                        switch (genericParameter.Independence)
                        {
                            default:
                                throw ExhaustiveMatch.Failed(genericParameter.Independence);
                            case TypeParameterIndependence.Independent:
                            {
                                if (from is not CapabilityType fromCapabilityType
                                    || to is not CapabilityType toCapabilityType)
                                    return false;

                                if (fromCapabilityType.BareType != toCapabilityType.BareType
                                    // TODO does this handle `iso` and `id` correctly?
                                    || !toCapabilityType.Capability.IsAssignableFrom(fromCapabilityType.Capability))
                                    return false;
                                break;
                            }
                            case TypeParameterIndependence.SharableIndependent:
                            {
                                if (from is not CapabilityType fromCapabilityType
                                    || to is not CapabilityType toCapabilityType)
                                    return false;

                                if (fromCapabilityType.BareType != toCapabilityType.BareType
                                    // TODO does this handle `iso` and `id` correctly?
                                    || !toCapabilityType.Capability.IsAssignableFrom(fromCapabilityType.Capability))
                                    return false;

                                // Because `shareable ind` preserves the shareableness of the type, it cannot
                                // promote a `const` to `id`.
                                if (toCapabilityType.Capability == Capability.Identity
                                    && fromCapabilityType.Capability == Capability.Constant)
                                    return false;

                                // TODO what about `temp const`?
                                break;
                            }
                            case TypeParameterIndependence.None:
                                // Invariant and not independent, so not assignable when not equal
                                return false;
                        }
                    }
                    break;
                case ParameterVariance.NonwritableCovariant:
                    if (!targetAllowsWrite)
                        goto case ParameterVariance.Covariant;

                    goto case ParameterVariance.Invariant;
                case ParameterVariance.Covariant:
                    if (!to.IsAssignableFrom(from))
                        return false;
                    break;
                case ParameterVariance.Contravariant:
                    if (!from.IsAssignableFrom(to))
                        return false;
                    break;
            }
        }
        return true;
    }

    public static bool IsAssignableFrom(this FunctionType target, FunctionType source)
    {
        if (target.Parameters.Count != source.Parameters.Count)
            return false;

        foreach (var (targetParameter, sourceParameter) in target.Parameters.EquiZip(source.Parameters))
            if (!targetParameter.IsAssignableFrom(sourceParameter))
                return false;

        return IsAssignableFrom(target.Return, source.Return);
    }

    public static bool IsAssignableFrom(this ParameterType target, ParameterType source)
    {
        // TODO add more flexibility in lent
        if (target.IsLent != source.IsLent) return false;

        // Parameter types need to be more specific in the target than the source.
        return source.Type.IsAssignableFrom(target.Type);
    }

    public static bool IsAssignableFrom(this ReturnType target, ReturnType source)
        // Return types need to be more general in the target than the source.
        => target.Type.IsAssignableFrom(source.Type);

    /// <summary>
    /// Replace self viewpoint types using the given type as self.
    /// </summary>
    public static DataType ReplaceSelfWith(this DataType type, DataType selfType)
    {
        if (selfType is not CapabilityType selfReferenceType)
            return type;
        return type.ReplaceSelfWith(selfReferenceType.Capability);
    }

    /// <summary>
    /// Replace self viewpoint types using the given type as self.
    /// </summary>
    public static DataType ReplaceSelfWith(this DataType type, Capability capability)
    {
        return type switch
        {
            SelfViewpointType t => t.Referent.ReplaceSelfWith(capability).AccessedVia(capability),
            // TODO doesn't this need to apply to type arguments?
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


    /// <summary>
    /// If this is a reference type or an optional reference type, the underlying reference type.
    /// Otherwise, <see langword="null"/>.
    /// </summary>
    public static CapabilityType? UnderlyingReferenceType(this DataType type)
    {
        return type switch
        {
            CapabilityType { BareType: BareReferenceType } referenceType => referenceType,
            OptionalType { Referent: CapabilityType { BareType: BareReferenceType } referenceType } => referenceType,
            _ => null
        };
    }

    /// <summary>
    /// Determine what the common type for two numeric types for a numeric operator is.
    /// </summary>
    public static DataType? NumericOperatorCommonType(this DataType leftType, DataType rightType)
        => (leftType, rightType) switch
        {
            (_, NeverType) => DataType.Never,
            (NeverType, _) => DataType.Never,
            ({ IsFullyKnown: false }, _) => DataType.Unknown,
            (_, { IsFullyKnown: false }) => DataType.Unknown,
            (OptionalType { Referent: var left }, OptionalType { Referent: var right })
                => left.NumericOperatorCommonType(right)?.MakeOptional(),
            (OptionalType { Referent: var left }, _) => left.NumericOperatorCommonType(rightType)?.MakeOptional(),
            (_, OptionalType { Referent: var right }) => leftType.NumericOperatorCommonType(right)?.MakeOptional(),
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
}
