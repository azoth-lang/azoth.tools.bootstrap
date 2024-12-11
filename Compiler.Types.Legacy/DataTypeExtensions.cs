using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors.Contexts;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Parameters;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

public static class DataTypeExtensions
{
    /// <summary>
    /// Tests whether a place of the target type could be assigned a value of the source type.
    /// This does not account for implicit conversions, but does allow for borrowing
    /// and sharing. It also allows for isolated upgrading to mutable.
    /// </summary>
    public static bool IsAssignableFrom(this IMaybeType target, IMaybeType source)
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

        if (target.BareType.IsAssignableFrom(target.AllowsWrite, source.BareType))
            return true;

        // TODO remove hack to allow string to exist in both primitives and stdlib
        return target.Name == "String"
               && source.Name == "String"
               && source.TypeConstructor?.Context is NamespaceContext sc
               && sc.Namespace == NamespaceName.Global
               && target.TypeConstructor?.Context is NamespaceContext tc
               && tc.Namespace == NamespaceName.Global;
    }

    public static bool IsAssignableFrom(this BareType target, bool targetAllowsWrite, BareType source)
    {
        return (target, source) switch
        {
            (BareNonVariableType t, BareNonVariableType s) => t.IsAssignableFrom(targetAllowsWrite, s),
            _ => throw new NotImplementedException()
        };
    }

    /// <remarks>We currently support implicit boxing, so any bare type with the correct supertype
    /// is assignable.</remarks>
    public static bool IsAssignableFrom(
        this BareNonVariableType target,
        bool targetAllowsWrite,
        BareNonVariableType source)
    {
        if (source.Equals(target) || source.Supertypes.Contains(target))
            return true;

        if (target.AllowsVariance || target.HasIndependentTypeArguments)
        {
            var declaredType = target.TypeConstructor;
            var matchingSourceTypes = source.Supertypes.Prepend(source).Where(t => t.TypeConstructor.Equals(declaredType));
            foreach (var sourceType in matchingSourceTypes)
                if (IsAssignableFrom(declaredType, targetAllowsWrite, target.TypeArguments, sourceType.TypeArguments))
                    return true;
        }

        return false;
    }

    private static bool IsAssignableFrom(
        TypeConstructor typeConstructor,
        bool targetAllowsWrite,
        IFixedList<IMaybeType> target,
        IFixedList<IMaybeType> source)
    {
        Requires.That(target.Count == typeConstructor.Parameters.Count, nameof(target), "count must match count of typeConstructor generic parameters");
        Requires.That(source.Count == target.Count, nameof(source), "count must match count of target");
        for (int i = 0; i < typeConstructor.Parameters.Count; i++)
        {
            var from = source[i];
            var to = target[i];
            var parameter = typeConstructor.Parameters[i];
            switch (parameter.Variance)
            {
                default:
                    throw ExhaustiveMatch.Failed(parameter.Variance);
                case TypeParameterVariance.Invariant:
                    if (!from.Equals(to))
                    {
                        // When target allows write, acts invariant regardless of independence
                        if (targetAllowsWrite) return false;

                        switch (parameter.Independence)
                        {
                            default:
                                throw ExhaustiveMatch.Failed(parameter.Independence);
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
                case TypeParameterVariance.NonwritableCovariant:
                    if (!targetAllowsWrite)
                        goto case TypeParameterVariance.Covariant;

                    goto case TypeParameterVariance.Invariant;
                case TypeParameterVariance.Covariant:
                    if (!to.IsAssignableFrom(from))
                        return false;
                    break;
                case TypeParameterVariance.Contravariant:
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

    /// <summary>
    /// Replace self viewpoint types using the given type as self.
    /// </summary>
    public static IMaybeType ReplaceSelfWith(this IMaybeType type, IMaybeType selfType)
    {
        if (selfType is not CapabilityType selfReferenceType)
            return type;
        return type.ReplaceSelfWith(selfReferenceType.Capability);
    }

    /// <summary>
    /// Replace self viewpoint types using the given type as self.
    /// </summary>
    public static IMaybeType ReplaceSelfWith(this IMaybeType type, Capability capability)
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

    /// <summary>
    /// If this is a reference type or an optional reference type, the underlying reference type.
    /// Otherwise, <see langword="null"/>.
    /// </summary>
    public static CapabilityType? UnderlyingReferenceType(this IMaybeType type)
        => type switch
        {
            CapabilityType { Semantics: TypeSemantics.Reference } referenceType => referenceType,
            OptionalType { Referent: CapabilityType { Semantics: TypeSemantics.Reference } referenceType } => referenceType,
            _ => null
        };

    /// <summary>
    /// Determine what the common type for two numeric types for a numeric operator is.
    /// </summary>
    public static IMaybeType? NumericOperatorCommonType(this IMaybeType leftType, IMaybeType rightType)
        => (leftType, rightType) switch
        {
            (_, NeverType) => IType.Never,
            (NeverType, _) => IType.Never,
            (UnknownType, _) => IType.Unknown,
            (_, UnknownType) => IType.Unknown,
            (OptionalType { Referent: var left }, OptionalType { Referent: var right })
                => left.OptionalNumericOperatorCommonType(right),
            (OptionalType { Referent: var left }, _) => left.OptionalNumericOperatorCommonType(rightType),
            (_, OptionalType { Referent: var right }) => leftType.OptionalNumericOperatorCommonType(right),
            (CapabilityType { TypeConstructor: var left }, CapabilityType { TypeConstructor: var right })
                => left.NumericOperatorCommonType(right),
            _ => null,
        };

    private static IMaybeType? OptionalNumericOperatorCommonType(this IMaybeType leftType, IMaybeType rightType)
    {
        var commonType = leftType.NumericOperatorCommonType(rightType);
        return commonType is not null ? OptionalType.Create(commonType) : null;
    }

    /// <summary>
    /// Determine what the common type for two numeric types for a numeric operator is.
    /// </summary>
    internal static IMaybeType? NumericOperatorCommonType(
        this TypeConstructor? leftTypeConstructor,
        TypeConstructor? rightTypeConstructor)
        => (leftTypeConstructor, rightTypeConstructor) switch
        {
            (BigIntegerTypeConstructor left, IntegerTypeConstructor right)
                => left.IsSigned || right.IsSigned ? IType.Int : IType.UInt,
            (IntegerTypeConstructor left, BigIntegerTypeConstructor right)
                => left.IsSigned || right.IsSigned ? IType.Int : IType.UInt,
            (BigIntegerTypeConstructor left, IntegerLiteralTypeConstructor right)
                => left.IsSigned || right.IsSigned ? IType.Int : IType.UInt,
            (IntegerLiteralTypeConstructor left, BigIntegerTypeConstructor right)
                => left.IsSigned || right.IsSigned ? IType.Int : IType.UInt,
            (PointerSizedIntegerTypeConstructor left, PointerSizedIntegerTypeConstructor right)
                => left.IsSigned || right.IsSigned ? IType.Offset : IType.Size,
            (PointerSizedIntegerTypeConstructor { IsSigned: true }, IntegerLiteralTypeConstructor { IsInt16: true })
                => IType.Offset,
            (PointerSizedIntegerTypeConstructor { IsSigned: false }, IntegerLiteralTypeConstructor { IsUInt16: true })
                => IType.Size,
            (PointerSizedIntegerTypeConstructor left, IntegerLiteralTypeConstructor right)
                => left.IsSigned || right.IsSigned ? IType.Int : IType.UInt,
            (IntegerLiteralTypeConstructor { IsInt16: true }, PointerSizedIntegerTypeConstructor { IsSigned: true })
                => IType.Offset,
            (IntegerLiteralTypeConstructor { IsUInt16: true }, PointerSizedIntegerTypeConstructor { IsSigned: false })
                => IType.Size,
            (IntegerLiteralTypeConstructor left, PointerSizedIntegerTypeConstructor right)
                => left.IsSigned || right.IsSigned ? IType.Int : IType.UInt,
            (FixedSizeIntegerTypeConstructor left, FixedSizeIntegerTypeConstructor right)
                when left.IsSigned == right.IsSigned
                => (left.Bits >= right.Bits ? left : right).TryConstructConstNullary(),
            (FixedSizeIntegerTypeConstructor { IsSigned: true } left, FixedSizeIntegerTypeConstructor right)
                when left.Bits > right.Bits
                => left.TryConstructConstNullary(),
            (FixedSizeIntegerTypeConstructor left, FixedSizeIntegerTypeConstructor { IsSigned: true } right)
                when left.Bits < right.Bits
                => right.TryConstructConstNullary(),
            (FixedSizeIntegerTypeConstructor { IsSigned: true } left, IntegerLiteralTypeConstructor right)
                when left.IsSigned || right.IsSigned
                => left.NumericOperatorCommonType(right.ToSmallestSignedIntegerType()),
            (FixedSizeIntegerTypeConstructor { IsSigned: false } left, IntegerLiteralTypeConstructor { IsSigned: false } right)
                => left.NumericOperatorCommonType(right.ToSmallestUnsignedIntegerType()),
            (IntegerLiteralTypeConstructor left, FixedSizeIntegerTypeConstructor right)
                when left.IsSigned || right.IsSigned
                => left.ToSmallestSignedIntegerType().NumericOperatorCommonType(right),
            (IntegerLiteralTypeConstructor { IsSigned: false } left, FixedSizeIntegerTypeConstructor { IsSigned: false } right)
                => left.ToSmallestSignedIntegerType().NumericOperatorCommonType(right),
            _ => null
        };
}
