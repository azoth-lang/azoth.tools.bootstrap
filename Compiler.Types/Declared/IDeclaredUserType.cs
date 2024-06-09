using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;
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
    StandardName Name { get; }
    IFixedList<GenericParameter> GenericParameters { get; }
    bool HasIndependentGenericParameters { get; }
    bool AllowsVariance { get; }
    IFixedList<GenericParameterType> GenericParameterTypes { get; }
    bool IsGeneric { get; }
    IFixedSet<BareReferenceType> Supertypes { get; }

    DeclaredType AsDeclaredType();

    BareType With(IFixedList<DataType> typeArguments);
    CapabilityType With(Capability capability, IFixedList<DataType> typeArguments);
    CapabilityTypeConstraint With(CapabilitySet capability, IFixedList<DataType> typeArguments);
    CapabilityType WithRead(IFixedList<DataType> typeArguments);

    IDeclaredAntetype ToAntetype();
}

internal static class DeclaredUserTypeExtensions
{
    /// <remarks>Used inside of instances of <see cref="IDeclaredUserType"/> to construct the
    /// equivalent <see cref="IDeclaredAntetype"/>. Do not use directly. Use
    /// <see cref="IDeclaredUserType.ToAntetype"/> instead.</remarks>
    internal static IDeclaredAntetype ConstructDeclaredAntetype(this IDeclaredUserType declaredType)
    {
        var antetypeGenericParameters = declaredType.GenericParameters.Select(p => new AntetypeGenericParameter(p.Name, p.Variance));
        var hasReferenceSemantics = declaredType is ObjectType;

        var supertypes = declaredType.Supertypes.Select(t => t.ToAntetype())
                                     .Cast<NominalAntetype>();
        return declaredType.Name switch
        {
            IdentifierName n
                => new UserNonGenericNominalAntetype(declaredType.ContainingPackage,
                    declaredType.ContainingNamespace, n, supertypes, hasReferenceSemantics),
            GenericName n
                => new UserDeclaredGenericAntetype(declaredType.ContainingPackage,
                    declaredType.ContainingNamespace, n, antetypeGenericParameters, supertypes, hasReferenceSemantics),
            _ => throw ExhaustiveMatch.Failed(declaredType.Name)
        };
    }
}
