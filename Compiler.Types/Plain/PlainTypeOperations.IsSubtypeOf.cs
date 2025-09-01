using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

// TODO either remove substitutable or replace with "allowBoxing"
public static partial class PlainTypeOperations
{
    /// <summary>
    /// Whether this <see cref="PlainType"/> is a subtype of the other <see cref="PlainType"/>. By
    /// default, this checks whether a type is a substitutable subtype of another. The
    /// <paramref name="substitutable"/> parameter can be used to include non-substitutable
    /// subtyping. Value types implement traits and are thus subtypes of them. However, because
    /// boxing is an explicit conversion, a value type cannot be directly passed where a trait it
    /// implements is expected. Thus value types are non-substitutable subtypes of traits they
    /// implement.
    /// </summary>
    public static bool IsSubtypeOf(this IMaybePlainType self, IMaybePlainType other, bool substitutable = true)
        => (self, other) switch
        {
            (UnknownPlainType, _) or (_, UnknownPlainType) => true,
            (PlainType s, PlainType o) => s.IsSubtypeOf(o, substitutable),
            _ => throw new UnreachableException()
        };

    /// <inheritdoc cref="IsSubtypeOf(IMaybePlainType,IMaybePlainType,bool)"/>
    public static bool IsSubtypeOf(this PlainType self, PlainType other, bool substitutable = true)
        => (self, other) switch
        {
            (_, _) when self.Equals(other) => true,
            // Never is even a subtype of void
            (NeverPlainType, _) => true,
            (VoidPlainType, _) => false,
            (OptionalPlainType s, OptionalPlainType o)
                => s.Referent.IsSubtypeOf(o.Referent, substitutable),
            (_, OptionalPlainType o)
                => self.Semantics == TypeSemantics.Reference && self.IsSubtypeOf(o.Referent, substitutable),
            (BarePlainType s, BarePlainType t) => s.IsSubtypeOf(t, substitutable),
            (FunctionPlainType s, FunctionPlainType o) => s.IsSubtypeOf(o),
            _ => false
        };

    /// <inheritdoc cref="IsSubtypeOf(IMaybePlainType,IMaybePlainType,bool)"/>
    public static bool IsSubtypeOf(
        this BarePlainType self,
        BarePlainType other,
        bool substitutable = true)
    {
        if (self.Equals(other))
            return true;
        if (self.Supertypes.Contains(other))
            return IsSubstitutable();

        // TODO remove hack to allow string to exist in both primitives and stdlib
        if (self.IsStringType() && other.IsStringType())
            return true;

        var otherTypeConstructor = other.TypeConstructor;
        if (otherTypeConstructor.AllowsVariance || otherTypeConstructor.HasIndependentParameters)
        {
            // Adding self covers cases where the types are identical except for parameters
            var selfPlainTypes = self.Supertypes.Prepend(self)
                                     .Where(t => otherTypeConstructor.Equals(t.TypeConstructor));
            if (selfPlainTypes.Any(selfPlainType => IsSubtypeOf(otherTypeConstructor, selfPlainType.Arguments, other.Arguments)))
            {
                return IsSubstitutable();
            }
        }

        return false;

        bool IsSubstitutable()
            => !substitutable
               || self.Semantics == TypeSemantics.Reference
               || self.Semantics == TypeSemantics.Value && other.Semantics == TypeSemantics.Value;
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
                    if (!self.IsSubtypeOf(other, substitutable: false))
                        return false;
                    break;
                case TypeParameterVariance.Contravariant:
                    if (!other.IsSubtypeOf(self, substitutable: false))
                        return false;
                    break;
            }
        }
        return true;
    }

    /// <summary>
    /// Whether one <see cref="FunctionPlainType"/> is a subtype of another <see cref="FunctionPlainType"/>.
    /// </summary>
    /// <remarks><see cref="FunctionPlainType"/>s never allow substitutability in their parameters
    /// or return types.</remarks>
    public static bool IsSubtypeOf(this FunctionPlainType self, FunctionPlainType other)
    {
        if (self.Parameters.Count != other.Parameters.Count)
            return false;

        foreach (var (selfParameter, otherParameter) in self.Parameters.EquiZip(other.Parameters))
            // Parameter types are contravariant
            if (!otherParameter.IsSubtypeOf(selfParameter, substitutable: false))
                return false;

        return self.Return.IsSubtypeOf(other.Return, substitutable: false);
    }
}
