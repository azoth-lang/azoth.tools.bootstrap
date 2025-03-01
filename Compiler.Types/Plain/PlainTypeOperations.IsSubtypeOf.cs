using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

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
            (UnknownPlainType, _) or (_, UnknownPlainType)
                => true,
            (PlainType s, PlainType o)
                => s.IsSubtypeOf(o, substitutable),
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
            (OptionalPlainType s, OptionalPlainType o) => s.Referent.IsSubtypeOf(o.Referent, substitutable),
            (_, OptionalPlainType o) => self.IsSubtypeOf(o.Referent, substitutable),
            (BarePlainType s, BarePlainType t) => s.IsSubtypeOf(t, substitutable),
            (FunctionPlainType s, FunctionPlainType o) => s.IsSubtypeOf(o),
            (RefPlainType s, RefPlainType o) => s.IsSubtypeOf(o, substitutable),
            _ => false
        };

    /// <inheritdoc cref="IsSubtypeOf(IMaybePlainType,IMaybePlainType,bool)"/>
    public static bool IsSubtypeOf(
        this BarePlainType self,
        BarePlainType other,
        bool substitutable = true)
    {
        // TODO apply substitutable
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

    /// <inheritdoc cref="IsSubtypeOf(IMaybePlainType,IMaybePlainType,bool)"/>
    public static bool IsSubtypeOf(this RefPlainType self, RefPlainType other, bool substitutable = true)
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
            return self.Referent.IsSubtypeOf(other.Referent);

        // If this method is directly called, then the case where they are equal must be covered
        return self.Equals(other);
    }
}
