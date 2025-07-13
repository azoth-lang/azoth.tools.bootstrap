using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

// TODO the logic here mostly duplicates the logic in PlainType.IsSubtypeOf is there a way to eliminate the duplication?
// TODO either remove substitutable or replace with "allowBoxing" (See Plain type operations)
public static partial class TypeOperations
{
    /// <summary>
    /// Whether <paramref name="self"/> is a subtype of <paramref name="other"/>.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="other"></param>
    /// <param name="substitutable">Whether the subtype must be substitutable for the supertype.
    /// By default, boxing is not allowed so a value type cannot be substituted for a trait. However,
    /// it is technically a subtype.</param>
    public static bool IsSubtypeOf(this IMaybeType self, IMaybeType other, bool substitutable = true)
        => (self, other) switch
        {
            (UnknownType, _) or (_, UnknownType) => true,
            (Type s, Type o) => s.IsSubtypeOf(o, substitutable),
            _ => throw new UnreachableException()
        };

    /// <inheritdoc cref="IsSubtypeOf(IMaybeType,IMaybeType,bool)"/>
    public static bool IsSubtypeOf(this Type self, Type other, bool substitutable = true)
        => (self, other) switch
        {
            (_, _) when self.Equals(other) => true,
            (NeverType, _) => true,
            (CapabilityType s, CapabilityType o)
                => s.IsSubtypeOf(o, substitutable),
            (CapabilityType s, SelfViewpointType o)
                => s.IsSubtypeOf(o, substitutable),
            (CapabilityType s, CapabilitySetSelfType o)
                => s.IsSubtypeOf(o, substitutable),
            (SelfViewpointType s, CapabilityType o)
                => s.IsSubtypeOf(o, substitutable),
            (SelfViewpointType s, SelfViewpointType o)
                => s.IsSubtypeOf(o, substitutable),
            (OptionalType s, OptionalType o)
                => s.Referent.IsSubtypeOf(o.Referent, substitutable),
            (_, OptionalType o)
                => self.Semantics == TypeSemantics.Reference && self.IsSubtypeOf(o.Referent, substitutable),
            (FunctionType s, FunctionType o)
                => s.IsSubtypeOf(o),
            (RefType s, RefType o) => s.IsSubtypeOf(o),
            _ => false,
        };

    /// <inheritdoc cref="IsSubtypeOf(IMaybeType,IMaybeType,bool)"/>
    public static bool IsSubtypeOf(this CapabilityType self, CapabilityType other, bool substitutable = true)
        => self.Capability.IsSubtypeOf(other.Capability)
           && self.BareType.IsSubtypeOf(other.BareType, other.Capability.AllowsWrite, substitutable);

    /// <inheritdoc cref="IsSubtypeOf(IMaybeType,IMaybeType,bool)"/>
    public static bool IsSubtypeOf(this CapabilityType self, SelfViewpointType other, bool substitutable = true)
    {
        if (!self.Capability.IsSubtypeOf(other.CapabilitySet)) return false;

        // TODO this is incorrect, it doesn't account for capabilities in the referent
        return self.PlainType.IsSubtypeOf(other.PlainType, substitutable);
    }

    /// <inheritdoc cref="IsSubtypeOf(IMaybeType,IMaybeType,bool)"/>
    public static bool IsSubtypeOf(this CapabilityType self, CapabilitySetSelfType other, bool substitutable = true)
    {
        if (!self.Capability.IsSubtypeOf(other.CapabilitySet)) return false;

        return self.BareType.IsSubtypeOf(other.BareType, other.CapabilitySet.AnyCapabilityAllowsWrite, substitutable);
    }

    /// <inheritdoc cref="IsSubtypeOf(IMaybeType,IMaybeType,bool)"/>
    public static bool IsSubtypeOf(this SelfViewpointType self, CapabilityType other, bool substitutable = true)
        => self.CapabilitySet.IsSubtypeOf(other.Capability)
           // Capabilities on the referent also need to satisfy other.Capability
           && self.Referent.IsSubtypeOf(other, substitutable);

    /// <inheritdoc cref="IsSubtypeOf(IMaybeType,IMaybeType,bool)"/>
    public static bool IsSubtypeOf(this SelfViewpointType self, SelfViewpointType other, bool substitutable = true)
        => self.CapabilitySet.IsSubtypeOf(other.CapabilitySet)
           && self.Referent.IsSubtypeOf(other.Referent, substitutable);

    public static bool IsSubtypeOf(this BareType self, BareType other, bool otherAllowsWrite, bool substitutable = true)
    {
        if (self.Equals(other))
            return true;
        if (self.Supertypes.Contains(other) || other.Subtypes.Contains(self))
            return IsSubstitutable();

        // TODO remove hack to allow string to exist in both primitives and stdlib
        if (self.PlainType.IsStringType() && other.PlainType.IsStringType()) return true;

        var otherTypeConstructor = other.TypeConstructor;
        if (otherTypeConstructor.AllowsVariance || otherTypeConstructor.HasIndependentParameters)
        {
            // Adding self covers cases where the types are identical except for parameters
            var selfTypes = self.Supertypes.Prepend(self)
                                         .Where(s => s.TypeConstructor.Equals(otherTypeConstructor));
            if (selfTypes.Any(selfType => IsSubtypeOf(otherTypeConstructor, selfType.Arguments, other.Arguments, otherAllowsWrite)))
                return IsSubstitutable();
        }

        return false;

        bool IsSubstitutable()
            => !substitutable || self.Semantics == other.Semantics;
    }

    private static bool IsSubtypeOf(
        BareTypeConstructor typeConstructor,
        IFixedList<IMaybeType> self,
        IFixedList<IMaybeType> other,
        bool otherAllowsWrite)
    {
        Requires.That(other.Count == typeConstructor.Parameters.Count, nameof(other), "count must match count of typeConstructor generic parameters");
        Requires.That(self.Count == other.Count, nameof(self), "count must match count of other");
        for (int i = 0; i < typeConstructor.Parameters.Count; i++)
        {
            var from = self[i];
            var to = other[i];
            var parameter = typeConstructor.Parameters[i];
            switch (parameter.Variance)
            {
                default:
                    throw ExhaustiveMatch.Failed(parameter.Variance);
                case TypeParameterVariance.Invariant:
                    if (!from.Equals(to))
                    {
                        // When other allows write, acts invariant regardless of independence
                        if (otherAllowsWrite) return false;

                        switch (parameter.Independence)
                        {
                            default:
                                throw ExhaustiveMatch.Failed(parameter.Independence);
                            case TypeParameterIndependence.Independent:
                            {
                                if (from is not CapabilityType fromCapabilityType
                                    || to is not CapabilityType toCapabilityType)
                                    return false;

                                if (!fromCapabilityType.BareType.Equals(toCapabilityType.BareType)
                                    // TODO does this handle `iso` and `id` correctly?
                                    || !toCapabilityType.Capability.IsAssignableFrom(fromCapabilityType.Capability))
                                    return false;
                                break;
                            }
                            case TypeParameterIndependence.ShareableIndependent:
                            {
                                if (from is not CapabilityType fromCapabilityType
                                    || to is not CapabilityType toCapabilityType)
                                    return false;

                                if (!fromCapabilityType.BareType.Equals(toCapabilityType.BareType)
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
                    if (!otherAllowsWrite)
                        goto case TypeParameterVariance.Covariant;

                    goto case TypeParameterVariance.Invariant;
                case TypeParameterVariance.Covariant:
                    if (!from.IsSubtypeOf(to, substitutable: true))
                        return false;
                    break;
                case TypeParameterVariance.Contravariant:
                    if (!to.IsSubtypeOf(from, substitutable: true))
                        return false;
                    break;
            }
        }
        return true;
    }

    /// <inheritdoc cref="IsSubtypeOf(IMaybeType,IMaybeType,bool)"/>
    public static bool IsSubtypeOf(this FunctionType self, FunctionType other)
    {
        if (self.Parameters.Count != other.Parameters.Count)
            return false;

        foreach (var (selfParameter, otherParameter) in self.Parameters.EquiZip(other.Parameters))
            if (!selfParameter.CanOverride(otherParameter))
                return false;

        return self.Return.ReturnCanOverride(other.Return);
    }

    /// <inheritdoc cref="IsSubtypeOf(IMaybeType,IMaybeType,bool)"/>
    public static bool IsSubtypeOf(this RefType self, RefType other)
    {
        // `iref var T <: ref var T`
        if ((self, other)
            is ({ IsInternal: true, IsMutableBinding: true }, { IsInternal: false, IsMutableBinding: true }))
            // Types must match because it can be assigned into
            return self.Referent.Equals(other.Referent);

        // `ref var S <: ref T`
        // `iref S <: ref T`
        // `iref var S <: ref T`
        // `iref var S <: iref T`
        // when S <: T
        if (!other.IsMutableBinding && other.IsInternal.Implies(self.IsInternal))
            return self.Referent.IsSubtypeOf(other.Referent, substitutable: true);

        // If this method is directly called, then the case where they are equal must be covered
        return self.Equals(other);
    }
}
