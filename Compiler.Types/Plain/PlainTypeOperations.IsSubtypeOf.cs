using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public static partial class PlainTypeOperations
{
    /// <summary>
    /// Whether this antetype is a subtype of the other antetype.
    /// </summary>
    public static bool IsSubtypeOf(this IMaybeAntetype self, IMaybeAntetype other)
    {
        return (self, other) switch
        {
            (UnknownPlainType, _) or (_, UnknownPlainType)
                => true,
            (IAntetype s, IAntetype o)
                => s.IsSubtypeOf(o),
            _ => throw new UnreachableException()
        };
    }

    public static bool IsSubtypeOf(this IAntetype self, IAntetype other)
    {
        return (self, other) switch
        {
            (_, _) when self.Equals(other) => true,
            (NeverPlainType, _) => true,
            (VoidPlainType, _) => false,
            (OptionalPlainType s, OptionalPlainType o) => s.Referent.IsSubtypeOf(o.Referent),
            (_, OptionalPlainType o) => self.IsSubtypeOf(o.Referent),
            (NamedPlainType s, NamedPlainType t) => s.IsSubtypeOf(t),
            (FunctionPlainType s, FunctionPlainType o) => s.IsSubtypeOf(o),
            _ => false
        };
    }

    public static bool IsSubtypeOf(
        this NamedPlainType self,
        NamedPlainType other)
    {
        if (self.Equals(other) || self.Supertypes.Contains(other))
            return true;

        // TODO remove hack to allow string to exist in both primitives and stdlib
        if (self is OrdinaryNamedPlainType s
           && other is OrdinaryNamedPlainType o
           && s.Name == "String" && o.Name == "String"
           && s.ContainingNamespace == NamespaceName.Global
           && o.ContainingNamespace == NamespaceName.Global)
            return true;

        var otherTypeConstructor = other.TypeConstructor;
        if (otherTypeConstructor is not null && other.AllowsVariance)
        {
            var selfAntetypes = self.Supertypes.Prepend(self)
                                           .Where(t => otherTypeConstructor.Equals(t.TypeConstructor));
            foreach (var selfAntetype in selfAntetypes)
                if (IsSubtypeOf(otherTypeConstructor, selfAntetype.TypeArguments, other.TypeArguments))
                    return true;
        }

        return false;
    }

    private static bool IsSubtypeOf(
        ITypeConstructor declaredAntetype,
        IFixedList<IAntetype> selfTypeArguments,
        IFixedList<IAntetype> otherTypeArguments)
    {
        Requires.That(selfTypeArguments.Count == declaredAntetype.Parameters.Count, nameof(selfTypeArguments), "count must match count of declaredAntetype generic parameters");
        Requires.That(otherTypeArguments.Count == declaredAntetype.Parameters.Count, nameof(otherTypeArguments), "count must match count of declaredAntetype generic parameters");
        for (int i = 0; i < declaredAntetype.Parameters.Count; i++)
        {
            var genericParameter = declaredAntetype.Parameters[i];
            var self = selfTypeArguments[i];
            var other = otherTypeArguments[i];
            switch (genericParameter.Variance)
            {
                default:
                    throw ExhaustiveMatch.Failed(genericParameter.Variance);
                case TypeVariance.Invariant:
                    if (!self.Equals(other))
                        return false;
                    break;
                case TypeVariance.Covariant:
                    if (!self.IsSubtypeOf(other))
                        return false;
                    break;
                case TypeVariance.Contravariant:
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
