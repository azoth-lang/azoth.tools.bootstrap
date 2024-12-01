using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;

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

    BareNonVariableType With(IFixedList<IType> typeArguments);
    CapabilityType With(Capability capability, IFixedList<IType> typeArguments);
    CapabilityTypeConstraint With(CapabilitySet capability, IFixedList<IType> typeArguments);
    CapabilityType WithRead(IFixedList<IType> typeArguments);
    public IMaybeExpressionType WithRead(IFixedList<IMaybeType> typeArguments)
    {
        var properTypeArguments = typeArguments.As<IType>();
        if (properTypeArguments is null) return IType.Unknown;
        return WithRead(properTypeArguments);
    }

    OrdinaryTypeConstructor ToTypeConstructor();
}

internal static class DeclaredUserTypeExtensions
{
    /// <remarks>Used inside of instances of <see cref="IDeclaredUserType"/> to construct the
    /// equivalent <see cref="ITypeConstructor"/>. Do not use directly. Use
    /// <see cref="IDeclaredUserType.ToTypeConstructor"/> instead.</remarks>
    internal static OrdinaryTypeConstructor ConstructTypeConstructor(this IDeclaredUserType declaredType)
    {
        var isAbstract = declaredType.IsAbstract;
        var antetypeGenericParameters = declaredType.GenericParameters
            // Treat self as non-writeable because antetypes should permit anything that could possibly be allowed by the types
            .Select(p => new TypeConstructorParameter(p.Name, p.Variance.ToTypeVariance(true)));
        var semantics = declaredType is ObjectType ? TypeSemantics.Reference : TypeSemantics.Value;
        var supertypes = declaredType.AntetypeSupertypes();
        return new OrdinaryTypeConstructor(declaredType.ContainingPackage, declaredType.ContainingNamespace,
            isAbstract, declaredType.Name, antetypeGenericParameters, supertypes, semantics);
    }

    private static IFixedSet<NominalAntetype> AntetypeSupertypes(this IDeclaredUserType declaredType)
        => declaredType.Supertypes.Select(t => t.ToAntetype()).Cast<NominalAntetype>().ToFixedSet();
}
