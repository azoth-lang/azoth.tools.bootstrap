using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Declared;

/// <summary>
/// A declared type that is not a simple type or the `Any` type. Thus it is declared by the "user".
/// </summary>
/// <remarks>Some intrinsic types also fall into this category.</remarks>
[Closed(typeof(ObjectType), typeof(StructType))]
public interface IDeclaredUserType : IEquatable<IDeclaredUserType>
{
    IdentifierName ContainingPackage { get; }
    NamespaceName ContainingNamespace { get; }
    bool IsDeclaredConst { get; }
    bool IsClass { get; }
    bool IsAbstract { get; }
    StandardName Name { get; }
    IFixedList<GenericParameter> GenericParameters { get; }
    bool HasIndependentGenericParameters { get; }
    bool AllowsVariance { get; }
    IFixedList<GenericParameterType> GenericParameterTypes { get; }
    bool IsGeneric { get; }
    IFixedSet<BareReferenceType> Supertypes { get; }

    public DeclaredType AsDeclaredType { get; }

    BareType With(IFixedList<Type> typeArguments);
    CapabilityType With(Capability capability, IFixedList<Type> typeArguments);
    CapabilityTypeConstraint With(CapabilitySet capability, IFixedList<Type> typeArguments);
    CapabilityType WithRead(IFixedList<Type> typeArguments);
    public IMaybeExpressionType WithRead(IFixedList<IMaybeExpressionType> typeArguments)
    {
        var properTypeArguments = typeArguments.As<Type>();
        if (properTypeArguments is null) return IType.Unknown;
        return WithRead(properTypeArguments);
    }

    IDeclaredAntetype ToAntetype();
}

internal static class DeclaredUserTypeExtensions
{
    /// <remarks>Used inside of instances of <see cref="IDeclaredUserType"/> to construct the
    /// equivalent <see cref="IDeclaredAntetype"/>. Do not use directly. Use
    /// <see cref="IDeclaredUserType.ToAntetype"/> instead.</remarks>
    internal static IDeclaredAntetype ConstructDeclaredAntetype(this IDeclaredUserType declaredType)
    {
        var isAbstract = declaredType.IsAbstract;
        var antetypeGenericParameters = declaredType.GenericParameters
            // Treat self as non-writeable because antetypes should permit anything that could possibly be allowed by the types
            .Select(p => new AntetypeGenericParameter(p.Name, p.Variance.ToTypeVariance(true)));
        var hasReferenceSemantics = declaredType is ObjectType;
        var supertypes = declaredType.AntetypeSupertypes();
        return declaredType.Name switch
        {
            IdentifierName n
                => new UserNonGenericNominalAntetype(declaredType.ContainingPackage,
                    declaredType.ContainingNamespace, isAbstract, n, supertypes, hasReferenceSemantics),
            GenericName n
                => new UserDeclaredGenericAntetype(declaredType.ContainingPackage,
                    declaredType.ContainingNamespace, isAbstract, n, antetypeGenericParameters,
                    supertypes, hasReferenceSemantics),
            _ => throw ExhaustiveMatch.Failed(declaredType.Name)
        };
    }

    private static IFixedSet<NominalAntetype> AntetypeSupertypes(this IDeclaredUserType declaredType)
        => declaredType.Supertypes.Select(t => t.ToAntetype()).Cast<NominalAntetype>().ToFixedSet();
}
