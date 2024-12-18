using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public static partial class TypeOperations
{
    public static bool IsSubtypeOf(this IMaybeType self, IMaybeType other)
        => (self, other) switch
        {
            (_, _) when self.Equals(other) => true,
            (UnknownType, _) or (_, UnknownType) => true,
            (NeverType, _) => true,
            (CapabilityType s, CapabilityType o)
                => s.IsSubtypeOf(o),
            (CapabilityType s, SelfViewpointType o)
                => s.IsSubtypeOf(o),
            (SelfViewpointType s, SelfViewpointType o)
                => s.IsSubtypeOf(o),
            (OptionalType s, OptionalType o)
                => s.Referent.IsSubtypeOf(o.Referent),
            (_, OptionalType o)
                => self.IsSubtypeOf(o.Referent),
            (FunctionType s, FunctionType o)
                => s.IsSubtypeOf(o),
            _ => false,
        };

    public static bool IsSubtypeOf(this CapabilityType self, CapabilityType other)
        => self.Capability.IsSubtypeOf(other.Capability)
           && self.BareType.IsSubtypeOf(other.BareType, other.Capability.AllowsWrite);

    public static bool IsSubtypeOf(this CapabilityType self, SelfViewpointType other)
    {
        if (!self.Capability.IsSubtypeOf(other.CapabilitySet)) return false;

        // TODO this is incorrect, it doesn't account for capabilities in the referent
        return self.PlainType.IsSubtypeOf(other.PlainType);
    }

    public static bool IsSubtypeOf(this SelfViewpointType self, SelfViewpointType other)
        => self.CapabilitySet.IsSubtypeOf(other.CapabilitySet)
           && self.Referent.IsSubtypeOf(other.Referent);

    private static bool IsSubtypeOf(this BareType self, BareType other, bool otherAllowsWrite)
    {
        if (self.Equals(other) || self.Supertypes.Contains(other))
            return true;

        // TODO remove hack to allow string to exist in both primitives and stdlib
        if (self.PlainType.IsStringType() && other.PlainType.IsStringType()) return true;

        var typeConstructor = other.TypeConstructor;
        if (typeConstructor.AllowsVariance || typeConstructor.HasIndependentParameters)
        {
            var matchingSupertypes
                = self.Supertypes
                      // Adding self covers cases where the types are identical except for parameters
                      .Prepend(self)
                      .Where(s => s.TypeConstructor.Equals(typeConstructor));
            return matchingSupertypes
                .Any(matchingSupertype => IsSubtypeOf(typeConstructor, matchingSupertype.Arguments, other.Arguments, otherAllowsWrite));
        }

        return false;
    }

    private static bool IsSubtypeOf(
        TypeConstructor typeConstructor,
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
                            case TypeParameterIndependence.SharableIndependent:
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
                    if (!from.IsSubtypeOf(to))
                        return false;
                    break;
                case TypeParameterVariance.Contravariant:
                    if (!to.IsSubtypeOf(from))
                        return false;
                    break;
            }
        }
        return true;
    }

    public static bool IsSubtypeOf(this FunctionType self, FunctionType other)
    {
        if (self.Parameters.Count != other.Parameters.Count)
            return false;

        foreach (var (selfParameter, otherParameter) in self.Parameters.EquiZip(other.Parameters))
            if (!selfParameter.CanOverride(otherParameter))
                return false;

        return self.Return.ReturnCanOverride(other.Return);
    }
}
