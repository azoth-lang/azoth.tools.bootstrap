using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public static partial class PlainTypeOperations
{
    /// <summary>
    /// Whether this plainType is a subtype of the other plainType.
    /// </summary>
    public static bool IsSubtypeOf(this IMaybePlainType self, IMaybePlainType other)
        => (self, other) switch
        {
            (UnknownPlainType, _) or (_, UnknownPlainType)
                => true,
            (PlainType s, PlainType o)
                => s.IsSubtypeOf(o),
            _ => throw new UnreachableException()
        };

    public static bool IsSubtypeOf(this PlainType self, PlainType other)
        => (self, other) switch
        {
            (_, _) when self.Equals(other) => true,
            (NeverPlainType, _) => true,
            (VoidPlainType, _) => false,
            (OptionalPlainType s, OptionalPlainType o) => s.Referent.IsSubtypeOf(o.Referent),
            (_, OptionalPlainType o) => self.IsSubtypeOf(o.Referent),
            (BarePlainType s, BarePlainType t) => s.IsSubtypeOf(t),
            (FunctionPlainType s, FunctionPlainType o) => s.IsSubtypeOf(o),
            _ => false
        };

    public static bool IsSubtypeOf(
        this BarePlainType self,
        BarePlainType other)
    {
        if (self.Equals(other) || self.Supertypes.Contains(other))
            return true;

        // TODO remove hack to allow string to exist in both primitives and stdlib
        if (self.IsStringType() && other.IsStringType())
            return true;

        var otherTypeConstructor = other.TypeConstructor;
        if (other.AllowsVariance)
        {
            var selfPlainTypes = self.Supertypes.Prepend(self)
                                    .Where(t => otherTypeConstructor.Equals(t.TypeConstructor));
            foreach (var selfPlainType in selfPlainTypes)
                if (IsSubtypeOf(otherTypeConstructor, selfPlainType.Arguments, other.Arguments))
                    return true;
        }

        return false;
    }

    private static bool IsSubtypeOf(
        BareTypeConstructor declaredPlainType,
        IFixedList<PlainType> selfTypeArguments,
        IFixedList<PlainType> otherTypeArguments)
    {
        Requires.That(selfTypeArguments.Count == declaredPlainType.Parameters.Count, nameof(selfTypeArguments), "count must match count of declaredPlainType generic parameters");
        Requires.That(otherTypeArguments.Count == declaredPlainType.Parameters.Count, nameof(otherTypeArguments), "count must match count of declaredPlainType generic parameters");
        for (int i = 0; i < declaredPlainType.Parameters.Count; i++)
        {
            var genericParameter = declaredPlainType.Parameters[i];
            var self = selfTypeArguments[i];
            var other = otherTypeArguments[i];
            switch (genericParameter.Variance)
            {
                default:
                    throw ExhaustiveMatch.Failed(genericParameter.Variance);
                case TypeParameterVariance.Invariant:
                // Since plain types are always writeable, NonwritableCovariant always acts as invariant
                case TypeParameterVariance.NonwritableCovariant:
                    if (!self.Equals(other))
                        return false;
                    break;
                case TypeParameterVariance.Covariant:
                    if (!self.IsSubtypeOf(other))
                        return false;
                    break;
                case TypeParameterVariance.Contravariant:
                    if (!other.IsSubtypeOf(self))
                        return false;
                    break;
            }
        }
        return true;
    }

    public static bool IsSubtypeOf(this FunctionPlainType self, FunctionPlainType other)
    {
        if (self.Parameters.Count != other.Parameters.Count)
            return false;

        foreach (var (selfParameter, otherParameter) in self.Parameters.EquiZip(other.Parameters))
            // Parameter types are contravariant
            if (!otherParameter.IsSubtypeOf(selfParameter))
                return false;

        return self.Return.IsSubtypeOf(other.Return);
    }
}
