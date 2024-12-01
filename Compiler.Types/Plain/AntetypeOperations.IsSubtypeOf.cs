using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public static partial class AntetypeOperations
{
    /// <summary>
    /// Whether this antetype is a subtype of the other antetype.
    /// </summary>
    public static bool IsSubtypeOf(this IMaybeExpressionAntetype self, IMaybeExpressionAntetype other)
    {
        return (self, other) switch
        {
            (UnknownAntetype, _) or (_, UnknownAntetype)
                => true,
            (IExpressionAntetype s, IExpressionAntetype o)
                => s.IsSubtypeOf(o),
            _ => throw new UnreachableException()
        };
    }

    public static bool IsSubtypeOf(this IExpressionAntetype self, IExpressionAntetype other)
    {
        return (self, other) switch
        {
            (_, _) when self.Equals(other) => true,
            (NeverAntetype, _) => true,
            (VoidAntetype, _) => false,
            (OptionalAntetype s, OptionalAntetype o) => s.Referent.IsSubtypeOf(o.Referent),
            (_, OptionalAntetype o) => self.IsSubtypeOf(o.Referent),
            (INonVoidAntetype and not OptionalAntetype, AnyAntetype)
                // Optional types are not subtypes of `Any`. But because of boxing, any non-void type is a subtype of `Any`.
                => true,
            (NominalAntetype s, NominalAntetype t) => s.IsSubtypeOf(t),
            (FunctionAntetype s, FunctionAntetype o) => s.IsSubtypeOf(o),
            _ => false
        };
    }

    public static bool IsSubtypeOf(
        this NominalAntetype self,
        NominalAntetype other)
    {
        if (self.Equals(other) || self.Supertypes.Contains(other))
            return true;

        // TODO remove hack to allow string to exist in both primitives and stdlib
        if (self is UserNonGenericNominalAntetype s
           && other is UserNonGenericNominalAntetype o
           && s.Name == "String" && o.Name == "String"
           && s.ContainingNamespace == NamespaceName.Global
           && o.ContainingNamespace == NamespaceName.Global)
            return true;

        if (other.AllowsVariance)
        {
            var otherDeclaredAntetype = other.DeclaredAntetype;
            var selfAntetypes = self.Supertypes.Prepend(self)
                                           .Where(t => t.DeclaredAntetype.Equals(otherDeclaredAntetype));
            foreach (var selfAntetype in selfAntetypes)
                if (IsSubtypeOf(otherDeclaredAntetype, selfAntetype.TypeArguments, other.TypeArguments))
                    return true;
        }

        return false;
    }

    private static bool IsSubtypeOf(
        ITypeConstructor declaredAntetype,
        IFixedList<IAntetype> selfTypeArguments,
        IFixedList<IAntetype> otherTypeArguments)
    {
        Requires.That(selfTypeArguments.Count == declaredAntetype.GenericParameters.Count, nameof(selfTypeArguments), "count must match count of declaredAntetype generic parameters");
        Requires.That(otherTypeArguments.Count == declaredAntetype.GenericParameters.Count, nameof(otherTypeArguments), "count must match count of declaredAntetype generic parameters");
        for (int i = 0; i < declaredAntetype.GenericParameters.Count; i++)
        {
            var genericParameter = declaredAntetype.GenericParameters[i];
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

    public static bool IsSubtypeOf(this FunctionAntetype self, FunctionAntetype other)
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
